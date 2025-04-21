using Azure.Messaging.ServiceBus;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string requestQueueName = "message-patterns.request-reply.request";

// Let the user know which application is running
Console.WriteLine("Responder application is running.");
Console.WriteLine();

// Create a ServiceBusClient that you can use to create a ServiceBusReceiver
await using ServiceBusClient client = new(connectionString);

// Create a processor for the request queue
ServiceBusProcessor serviceBusProcessor = client.CreateProcessor(requestQueueName);

// Register a handler for processing messages
serviceBusProcessor.ProcessMessageAsync += async args =>
{

	ServiceBusReceivedMessage requestMessage = args.Message;
	Console.WriteLine();
	Console.WriteLine("==== Request Received ====");
	Console.WriteLine($"CorrelationId: {requestMessage.CorrelationId}");
	Console.WriteLine($"ReplyTo: {requestMessage.ReplyTo}");
	Console.WriteLine($"Body: {requestMessage.Body}");

	ServiceBusMessage responseMessage = new("The time is " + DateTime.Now)
	{
		CorrelationId = requestMessage.CorrelationId
	};

	string replyTo = requestMessage.ReplyTo;
	if (!string.IsNullOrEmpty(replyTo))
	{
		await using ServiceBusSender responseSender = client.CreateSender(replyTo);
		await responseSender.SendMessageAsync(responseMessage);
		Console.WriteLine();
		Console.WriteLine("==== Response Sent ====");
		Console.WriteLine($"CorrelationId: {responseMessage.CorrelationId}");
		Console.WriteLine($"Body: {responseMessage.Body}");
	}

	await args.CompleteMessageAsync(requestMessage);

};

// Register a handler for any errors that occur when processing messages
serviceBusProcessor.ProcessErrorAsync += args =>
{
	Console.WriteLine(args.Exception.ToString());
	return Task.CompletedTask;
};

// Start processing
await serviceBusProcessor.StartProcessingAsync();
Console.WriteLine($"Listening for messages on: {requestQueueName}. Press any key to exit.");
Console.ReadKey();

// Stop processing
await serviceBusProcessor.StopProcessingAsync();
Console.WriteLine();
Console.WriteLine("Stopped processing messages.");