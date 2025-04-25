using Azure.Messaging.ServiceBus;
using System.Text;

namespace SagaCoorindator;

public class SagaCoordinator(
	ServiceBusClient serviceBusClient,
	string orderQueueName,
	string paymentQueueName,
	string inventoryQueueName,
	string completionQueueName)
{

	private readonly ServiceBusClient _serviceBusClient = serviceBusClient;
	private readonly string _orderQueueName = orderQueueName;
	private readonly string _paymentQueueName = paymentQueueName;
	private readonly string _inventoryQueueName = inventoryQueueName;
	private readonly string _completionQueueName = completionQueueName;

	public async Task StartSagaAsync(string orderId)
	{
		try
		{
			SagaAcknowledgements acknowledgements = new();

			Console.WriteLine($"Saga for {orderId} has been started");
			await SendMessageAsync(_orderQueueName, $"Create Order for {orderId}");
			await SendMessageAsync(_paymentQueueName, $"Charge Payment for {orderId}");
			await SendMessageAsync(_inventoryQueueName, $"Update Inventory for {orderId}");

			// Wait for the acknowledgements
			await using ServiceBusReceiver serviceBusReceiver = _serviceBusClient.CreateReceiver(_completionQueueName);
			while (!acknowledgements.SagaIsComplete)
			{
				ServiceBusReceivedMessage receivedMessage = await serviceBusReceiver.ReceiveMessageAsync();
				if (receivedMessage is not null)
				{
					string? serviceName = receivedMessage.ApplicationProperties["Service"].ToString();
					if (serviceName is not null)
					{
						bool success = (bool)receivedMessage.ApplicationProperties["Success"];
						Console.WriteLine($"Received acknowledgement from {serviceName}: {(success ? "Success" : "Failure")}");
						if (!success)
						{
							await CompenstateAsync(orderId);
							Console.WriteLine($"Saga for {orderId} has been aborted due to an error in {serviceName}");
							break;
						}
						switch (serviceName)
						{
							case "Order":
								acknowledgements.OrderAcknowledged = true;
								break;
							case "Payment":
								acknowledgements.PaymentAcknowledged = true;
								break;
							case "Inventory":
								acknowledgements.InventoryAcknowledged = true;
								break;
						}
						await serviceBusReceiver.CompleteMessageAsync(receivedMessage);
					}
				}
			}

			if (acknowledgements.SagaIsComplete)
				Console.WriteLine($"Saga for {orderId} has been completed");
			else
				Console.WriteLine($"Saga for {orderId} has been aborted due to an error");

		}
		catch (Exception ex)
		{
			await CompenstateAsync(orderId);
			Console.WriteLine($"Saga for {orderId} has been aborted due to an error: {ex.Message}");
		}
	}

	private async Task SendMessageAsync(string queueName, string message)
	{
		await using ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(queueName);
		await serviceBusSender.SendMessageAsync(new ServiceBusMessage(Encoding.UTF8.GetBytes(message)));
		Console.WriteLine($"Message sent to {queueName}: {message}");
	}

	private async Task CompenstateAsync(string orderId)
	{
		await SendMessageAsync(_inventoryQueueName, $"Revert Inventory for {orderId}");
		await SendMessageAsync(_paymentQueueName, $"Refund Payment for {orderId}");
		await SendMessageAsync(_orderQueueName, $"Cancel Order for {orderId}");
		Console.WriteLine($"Saga for {orderId} has been compensated");
	}

}