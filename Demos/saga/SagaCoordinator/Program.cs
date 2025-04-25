using Azure.Messaging.ServiceBus;
using SagaCoorindator;

const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string orderQueueName = "message-patterns.saga.order";
const string paymentQueueName = "message-patterns.saga.payment";
const string inventoryQueueName = "message-patterns.saga.inventory";
const string completionQueueName = "message-patterns.saga.completion";

// Create the ServiceBusClient
await using ServiceBusClient serviceBusClient = new(connectionString);

// Create the SagaCoordinator, OrderService, PaymentService, and InventoryService
SagaCoordinator sagaCoordinator = new(serviceBusClient, orderQueueName, paymentQueueName, inventoryQueueName, completionQueueName);

// Wait for the user to press a key before starting the saga
Console.WriteLine("Press any key to start the saga...");
Console.ReadKey(true);
Console.WriteLine();

// Start the saga
await sagaCoordinator.StartSagaAsync("123");

// Wait for the user to press a key before closing the console
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey(true);