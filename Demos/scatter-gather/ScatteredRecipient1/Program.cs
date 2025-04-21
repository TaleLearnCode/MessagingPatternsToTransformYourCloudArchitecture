using ScatteredRecipientHelper;

const string recipientName = "Scattered Recipient 1";
const string connectionString = "Endpoint=sb://127.0.0.1;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
const string topicName = "message-patterns.scatter-gather.request";
const string subscriptionName = "recipient-1";
const string gathererQueueName = "message-patterns.scatter-gather.gather";

await ScatteredRequestHelper.ProcessScatteredRequests(
	recipientName,
	connectionString,
	topicName,
	subscriptionName,
	gathererQueueName);