using Azure.Messaging.ServiceBus;
using System.Text;

namespace Core;

public static class ServiceHelper
{
	public static async Task SendAcknowledgementAsync(
		ServiceBusClient serviceBusClient,
		string completionQueueName,
		string service,
		bool success)
	{
		await using ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(completionQueueName);
		ServiceBusMessage serviceBusMessage = new(Encoding.UTF8.GetBytes("Acknolwedgement"));
		serviceBusMessage.ApplicationProperties.Add("Service", service);
		serviceBusMessage.ApplicationProperties.Add("Success", success);
		await serviceBusSender.SendMessageAsync(serviceBusMessage);
		Console.WriteLine($"Sent Acknowledgement for {service} : {(success ? "Success" : "Failure")}");
	}
}