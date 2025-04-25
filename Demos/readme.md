# Messaging Patterns Labs

Welcome to the **Messaging Patterns Labs** repository! This project contains hands-on labs demonstrating different messaging patterns using Azure Service Bus and .NET. These labs are designed to help you understand and implement common messaging patterns in distributed systems. The labs included are:

- **Request/Reply Messaging Pattern**
- **Scatter-Gather Messaging Pattern**
- **Saga Messaging Pattern**

Each lab walks you through the process of building and running a sample application that implements the pattern, providing detailed instructions, sample code, and explanations of the key concepts.

## Labs Overview

### 1. Request/Reply Messaging Pattern

**Overview:**  
This lab demonstrates a synchronous communication model where one component (the requester) sends a message and waits for an immediate reply from the responder. The Request/Reply pattern is helpful for scenarios requiring immediate response, such as querying data or confirming an action.

**Learn more & Run the Lab:**  
- [Request/Reply Lab Documentation](request-reply\README.md)

---

### 2. Scatter-Gather Messaging Pattern

**Overview:**  
The Scatter-Gather pattern distributes a request to multiple services (scattered recipients), which process it concurrently. The requester subsequently collects and aggregates the responses. This asynchronous pattern is ideal for tasks that can be processed in parallel and combined, such as distributed searches or data processing.

**Learn more & Run the Lab:**  
- [Scatter-Gather Lab Documentation](scatter-gather\README.md)

---

### 3. Saga Messaging Pattern

**Overview:**  
The Saga pattern orchestrates a long-running, multi-step business process across multiple services. Instead of a distributed transaction, a saga breaks the process into a series of local transactions, each committed independently. The Saga Coordinator handles the sequential execution of these transactions and triggers compensating actions if any step fails. This pattern is essential in microservices architectures, where maintaining consistency without distributed transactions is required.

**Learn more & Run the Lab:**  
- [Saga Lab Documentation](saga\README.md)

---

## Prerequisites

Before running the labs, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or a later compatible version)
- [Azure Service Bus Emulator](https://learn.microsoft.com/en-us/azure/service-bus-messaging/overview-emulator) configured with the provided `config.json` (or an Azure Service Bus namespace)
- Basic familiarity with C# and asynchronous programming patterns

## Getting Started

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/TaleLearnCode/MessagingPatternsToTransformYourCloudArchitecture.git
   cd MessagingPatternsToTransformYourCloudArchitecture
   ```

2. **Review Each Lab's README**: Each lab contains its own README file with details setup instructions, including how to configure the Service Bus emulator, build the project, and run the sample.

   - Request/Request: [request-reply/README.md](request-reply\README.md)
   - Scatter-Gather: [scatter-gather/README.md](scatter-gather\README.md)
   - Saga: [saga/README.md](saga\README.md)

3. **Configure Your Environment**: Ensure the Azure Service Bus Emulator runs and is configured using the provided `config.json` files in each lab's documentation.

## Additional Resources

- [Azure.Messaging.ServiceBus Documentation](https://docs.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus)
- [Distributed System Messaging Patterns](https://martinfowler.com/articles/microservices.html#asynchronous-inter-process-communication)
- [Microsoft Azure Service Bus Overview](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
