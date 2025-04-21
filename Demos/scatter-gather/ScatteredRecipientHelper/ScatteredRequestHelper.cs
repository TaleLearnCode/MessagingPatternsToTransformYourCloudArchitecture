using Azure.Messaging.ServiceBus;

namespace ScatteredRecipientHelper;

public static class ScatteredRequestHelper
{

	public static async Task ProcessScatteredRequests(
		string recipientName,
		string serviceBusConnectionString,
		string scatterTopicName,
		string recipientSubscriptionName,
		string gathererQueueName)
	{

		Random random = new();

		// Create a ServiceBusClient to be able to connect to the Service Bus namespace
		await using ServiceBusClient serviceBusClient = new(serviceBusConnectionString);

		// Create a ServiceBusProcessor in order to receive the requests
		await using ServiceBusProcessor requestProcessor = serviceBusClient.CreateProcessor(scatterTopicName, recipientSubscriptionName);

		// Create a ServiceBusSender in order to send the response
		await using ServiceBusSender responseSender = serviceBusClient.CreateSender(gathererQueueName);

		// Add an event handler to process messages
		requestProcessor.ProcessMessageAsync += async args =>
		{
			string body = args.Message.Body.ToString();
			Console.WriteLine($"Received message: {body}");

			// Send a response message to the gatherer queue
			ServiceBusMessage responseMessage = new("Scatter Recipient Response");
			responseMessage.ApplicationProperties["RecipientName"] = recipientName;
			responseMessage.ApplicationProperties["Response"] = random.Next(1, 100);
			await responseSender.SendMessageAsync(responseMessage);
			Console.WriteLine($"Sent recipient response message: {responseMessage.Body}");

			await args.CompleteMessageAsync(args.Message);
		};

		// Add an event handler to process any errors
		requestProcessor.ProcessErrorAsync += args =>
		{
			Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		};

		// Start processing
		await requestProcessor.StartProcessingAsync();
		Console.WriteLine();
		Console.WriteLine($"{recipientName} is listening for messages; press any key to stop...");
		Console.ReadKey(true);

		// Stop processing
		await requestProcessor.StopProcessingAsync();

	}

}