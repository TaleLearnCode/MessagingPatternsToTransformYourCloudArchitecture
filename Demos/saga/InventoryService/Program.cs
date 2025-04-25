using Azure.Messaging.ServiceBus;
using Core;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string ivnentoryQueueName = "message-patterns.saga.inventory";
const string completionQueueName = "message-patterns.saga.completion";

await using ServiceBusClient client = new(connectionString);
await using ServiceBusProcessor serviceBusProcessor = client.CreateProcessor(ivnentoryQueueName);

// Register the message handler
serviceBusProcessor.ProcessMessageAsync += async args =>
{
	Console.WriteLine();
	string body = args.Message.Body.ToString();
	Console.WriteLine($"Updating inventory: {body}");
	// Update the inventory
	await args.CompleteMessageAsync(args.Message);
	await ServiceHelper.SendAcknowledgementAsync(client, completionQueueName, "Inventory", true);
};

// Register the error handler
serviceBusProcessor.ProcessErrorAsync += args =>
{
	Console.WriteLine(args.Exception.ToString());
	return Task.CompletedTask;
};

// Start processing
await serviceBusProcessor.StartProcessingAsync();
Console.WriteLine("Processing Inventory messages. Press any key to exit...");
Console.ReadKey(true);

// Stop processing
await serviceBusProcessor.StopProcessingAsync();
Console.WriteLine("Stopped processing Inventory messages.");