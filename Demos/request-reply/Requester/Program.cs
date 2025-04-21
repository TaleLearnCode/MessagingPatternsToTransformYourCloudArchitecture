using Azure.Messaging.ServiceBus;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;"; ;
const string requestQueueName = "message-patterns.request-reply.request";
const string responseQueueName = "message-patterns.request-reply.response";

// Let the user know which application is running
Console.WriteLine("Requester application is running.");
Console.WriteLine();

// Prompt the user to press a key to send a request message
Console.WriteLine("Press any key to send a request message...");
Console.ReadKey();

// Create a ServiceBusClient that you can use to create a ServiceBusSender
await using ServiceBusClient client = new(connectionString);

// Create a sender for the request queue
ServiceBusSender requestSender = client.CreateSender(requestQueueName);

// Create a receiver for the response queue
ServiceBusReceiver responseReceiver = client.CreateReceiver(responseQueueName);

// Create a message that we will send to the request queue
ServiceBusMessage requestMessage = new("Request: What is the time?")
{
	ReplyTo = responseQueueName,
	CorrelationId = Guid.NewGuid().ToString()
};

// Send the message to the request queue
await requestSender.SendMessageAsync(requestMessage);
Console.WriteLine();
Console.WriteLine("==== Request Sent ====");
Console.WriteLine($"CorrelationId: {requestMessage.CorrelationId}");
Console.WriteLine($"ReplyTo: {requestMessage.ReplyTo}");
Console.WriteLine($"Body: {requestMessage.Body}");

// Receive the response from the response queue
ServiceBusReceivedMessage responseMessage = await responseReceiver.ReceiveMessageAsync();
Console.WriteLine();
Console.WriteLine("==== Response Received ====");
Console.WriteLine($"CorrelationId: {responseMessage.CorrelationId}");
Console.WriteLine($"Body: {responseMessage.Body}");