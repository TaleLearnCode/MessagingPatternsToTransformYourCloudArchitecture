using Azure.Messaging.ServiceBus;
using Core;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string paymentQueueName = "message-patterns.saga.payment";
const string completionQueueName = "message-patterns.saga.completion";

await using ServiceBusClient client = new(connectionString);
await using ServiceBusProcessor serviceBusProcessor = client.CreateProcessor(paymentQueueName);

// Register the message handler
serviceBusProcessor.ProcessMessageAsync += async args =>
{
	Console.WriteLine();
	string body = args.Message.Body.ToString();
	Console.WriteLine($"Processing payment: {body}");
	// Process the payment
	await args.CompleteMessageAsync(args.Message);
	await ServiceHelper.SendAcknowledgementAsync(client, completionQueueName, "Payment", true);
};

// Register the error handler
serviceBusProcessor.ProcessErrorAsync += args =>
{
	Console.WriteLine(args.Exception.ToString());
	return Task.CompletedTask;
};

// Start processing
await serviceBusProcessor.StartProcessingAsync();
Console.WriteLine("Processing Payment messages. Press any key to exit...");
Console.ReadKey(true);

// Stop processing
await serviceBusProcessor.StopProcessingAsync();
Console.WriteLine("Stopped processing Payment messages.");