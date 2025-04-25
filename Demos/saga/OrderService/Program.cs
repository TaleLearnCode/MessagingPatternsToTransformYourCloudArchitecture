using Azure.Messaging.ServiceBus;
using Core;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string orderQueueName = "message-patterns.saga.order";
const string completionQueueName = "message-patterns.saga.completion";

await using ServiceBusClient client = new(connectionString);
await using ServiceBusProcessor serviceBusProcessor = client.CreateProcessor(orderQueueName);

// Register the message handler
serviceBusProcessor.ProcessMessageAsync += async args =>
{
	Console.WriteLine();
	string body = args.Message.Body.ToString();
	Console.WriteLine($"Processing order: {body}");
	// Process the order
	await args.CompleteMessageAsync(args.Message);
	await ServiceHelper.SendAcknowledgementAsync(client, completionQueueName, "Order", true);
};

// Register the error handler
serviceBusProcessor.ProcessErrorAsync += args =>
{
	Console.WriteLine(args.Exception.ToString());
	return Task.CompletedTask;
};

// Start processing
await serviceBusProcessor.StartProcessingAsync();
Console.WriteLine("Processing Order messages. Press any key to exit...");

// Stop processing
await serviceBusProcessor.StopProcessingAsync();
Console.WriteLine("Stopped processing Order messages.");