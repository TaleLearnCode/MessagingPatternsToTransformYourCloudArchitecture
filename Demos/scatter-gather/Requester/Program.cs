using Azure.Messaging.ServiceBus;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string requestTopicName = "message-patterns.scatter-gather.request";
const string responseQueueName = "message-patterns.scatter-gather.response";

await SendRequestAsync();
await ReceiveResponse();

static async Task SendRequestAsync()
{

	Console.WriteLine("Press any key to start the scatter-gather process...");
	Console.ReadKey(true);

	// Create a Service Bus client and sender
	await using ServiceBusClient serviceBusClient = new(connectionString);
	await using ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(requestTopicName);

	// Send a scatter request message
	ServiceBusMessage message = new("Scatter Request");
	await serviceBusSender.SendMessageAsync(message);

	Console.WriteLine("Scatter Request sent.");

}

static async Task ReceiveResponse()
{

	// Create a Service Bus client and processor
	await using ServiceBusClient serviceBusClient = new(connectionString);
	await using ServiceBusProcessor serviceBusProcessor = serviceBusClient.CreateProcessor(responseQueueName);

	// Register the message handler
	serviceBusProcessor.ProcessMessageAsync += args =>
	{
		Console.WriteLine($"Received response: {args.Message.Body}");
		return Task.CompletedTask;
	};

	// Register the error handler
	serviceBusProcessor.ProcessErrorAsync += args =>
	{
		Console.WriteLine($"Error: {args.Exception.Message}");
		return Task.CompletedTask;
	};

	// Start processing messages
	await serviceBusProcessor.StartProcessingAsync();

	// Wait for user input to stop the receiver
	Console.WriteLine("Press any key to stop the receiver...");
	Console.ReadKey();
	await serviceBusProcessor.StopProcessingAsync();

	Console.WriteLine("Receiver stopped.");
	Console.ReadKey(true);

}