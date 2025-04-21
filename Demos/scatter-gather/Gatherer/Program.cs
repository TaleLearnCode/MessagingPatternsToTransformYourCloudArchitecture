using Azure.Messaging.ServiceBus;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string gathererQueueName = "message-patterns.scatter-gather.gather";
const string responseQueueName = "message-patterns.scatter-gather.response";

List<ServiceBusReceivedMessage> gatheredResponses = await GatherResponsesAsync();
ServiceBusReceivedMessage? aggregatedResponse = AgregateResponses(gatheredResponses);
await SendResponseAsync(aggregatedResponse);
Console.WriteLine("Done");
Console.ReadKey(true);

static async Task<List<ServiceBusReceivedMessage>> GatherResponsesAsync()
{
	Console.WriteLine("Gathering responses...");
	await using ServiceBusClient serviceBusClient = new(connectionString);
	await using ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(gathererQueueName);
	List<ServiceBusReceivedMessage> gatheredResponses = new List<ServiceBusReceivedMessage>();
	while (gatheredResponses.Count < 3)
	{
		ServiceBusReceivedMessage response = await serviceBusReceiver.ReceiveMessageAsync();
		gatheredResponses.Add(response);
		await serviceBusReceiver.CompleteMessageAsync(response);
	}
	return gatheredResponses;
}

static ServiceBusReceivedMessage? AgregateResponses(List<ServiceBusReceivedMessage> gatheredResponses)
{
	Console.WriteLine("Aggregating responses...");
	return gatheredResponses
		.Where(msg => msg.ApplicationProperties.ContainsKey("Response"))
		.OrderBy(msg => (int)msg.ApplicationProperties["Response"])
		.FirstOrDefault();
}

static async Task SendResponseAsync(ServiceBusReceivedMessage? aggregatedResponse)
{
	Console.WriteLine("Sending response...");
	await using ServiceBusClient serviceBusClient = new(connectionString);
	await using ServiceBusSender responseSender = serviceBusClient.CreateSender(responseQueueName);

	ServiceBusMessage responseMessage = aggregatedResponse is null
		? new("No response received")
		: new($"The best response was from `{aggregatedResponse.ApplicationProperties["RecipientName"]}` with a response of `{aggregatedResponse.ApplicationProperties["Response"]}`");

	await responseSender.SendMessageAsync(responseMessage);
}

// Create a Service Bus client, processor, and sender
await using ServiceBusClient serviceBusClient = new(connectionString);
await using ServiceBusProcessor gathererProcessor = serviceBusClient.CreateProcessor(gathererQueueName);

await using ServiceBusSender responseSender = serviceBusClient.CreateSender(responseQueueName);