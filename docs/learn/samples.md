# Samples

Working code is an excellent way to learn how to use MassTransit features. The samples below show the capabilities of MassTransit, and can be cloned, forked, and explored to get a better understanding.

The new samples are standalone repositories, which use NuGet to pull dependencies exactly as a developerwould use MassTransit.

### Getting Started

This project is [part of the MassTransit documentation](https://masstransit-project.com/getting-started/). Refer to that link for details.

**If you're new to MassTransit, start with this sample to understand how MassTransit works**

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-GettingStarted)

### Sample Twitch

This sample was created along with the [Twitch/YouTube video series](/getting-started/live-coding).

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Twitch)

### Sample Library

This sample was created along with Season 2 of the [Twitch/YouTube video series](/getting-started/live-coding).

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Library)

### Sample ForkJoint

This sample was created along with Season 3 of the [Twitch/YouTube video series](/getting-started/live-coding).

Fork Joint is a fictional restaurant built during Season 3 of the MassTransit Live Code Video Series. You can [watch the episodes on YouTube](https://youtube.com/playlist?list=PLx8uyNNs1ri2JeyDGFWfCYyAjOB1GP-t1) and follow along by resetting to the various commits in the Git history.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-ForkJoint)

### Trashlantis

This sample was created to show how the in-memory outbox is used and ensures message delivery in the presence of transaction failures.

Clone the sample: [GitHub Repository](https://github.com/phatboyg/Trashlantis)

### Node (MassTransit in TypeScript)

This sample uses MassTransit (for .NET) combined with the [MassTransit (for JavaScript) NPM package](https://www.npmjs.com/package/masstransit-rabbitmq) to send requests from a node application and handle the subsequent response from a MassTransit Consumer (running in .NET). The services communicate via RabbitMQ (included in the `docker-compose.yml` file).

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Node)

### Scoped Filters

> .NET 5, ASP.NET

This sample uses an HTTP header named `Token` to pass credentials to an API Controller. That header is read into a scoped type (`Token`) using an action filter as part of the API request. The action method then uses the MassTransit request client to send a request to a consumer. Scoped message filters are configured for publish, send, and consume to transfer the header value (via the `Token` object in the container scope) to outbound messages, and then on the consumer side extract that MassTransit message header back into the _Token_ type in the consumer scope.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-ScopedFilters)

### Azure Functions

Shows how to use MassTransit with Azure Functions (v3).

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-AzureFunction)

### Job Consumers

Shows how to use the job consumers with Entity Framework Core.

Features used:
- Job Consumers
- Entity Framework Core

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-JobConsumers)

### Batch Processing using Sagas

Shows how to perform batch processing and tracking using sagas.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Batch)

### SignalR

This sample will show a variety of built in tools and techniques in MassTransit.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-SignalR)

### Request Response

This sample demonstrates how to create a client that sends a request to a service which responds with a response.

Features used:
- Request Client

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-RequestResponse)

### Shopping Cart

This was a fun sample, created in response to a [blog post][1] on how to send an email to a customer that abandoned a shopping cart. My response to that post is [located here][2].

Features used:
- Automatonymous
- Quartz

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-ShoppingWeb)

[1]: http://joshkodroff.com/2015/08/21/an-elegant-abandoned-cart-email-using-nservicebus/
[2]: http://blog.phatboyg.com/general/2015/09/12/sagas-state-machines-and-abandoned-carts.html

### Courier 

Courier is MassTransit's routing-slip implementation, which makes it possible to orchestrate distributed services into a business transaction. This sample demonstrates how to create and execute a routing slip, record routing slip events, and track transaction state using [Automatonymous](https://github.com/MassTransit/Automatonymous).

This sample includes multiple console applications, which can be started simultaneously, to observe how the services interact.

Features used:
- Courier
- Automatonymous

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Courier)

### Race Registration

This sample has multiple console applications, and a web API, allowing registrations to be submitted. The routing slip is tracked using a saga, and can compensate when an activity faults.

Features used:
- Courier
- Automatonymous

Clone the sample: [GitHub Repository](https://github.com/phatboyg/Demo-Registration)

### Quartz

Features used:
- Scheduling
- Quartz

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Quartz)

### Hangfire

Features used:
- Scheduling
- Hangfire

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Hangfire)

### Application Insights

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-ApplicationInsights)

### RabbitMQ Direct Exchange

Shows how to configure a consumer and a producer to use RabbitMQ direct exchange routing.

Features used:
- RabbitMQ

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Direct)

### Benchmark

Test the performance of MassTransit in your environment.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/MassTransit-Benchmark)


### RabbitMQ MQTT Consumer

Utilise RabbitMQ as a MQTT server and consume IOT data.

Clone the sample: [GitHub Repository](https://github.com/morganphilo/MassTransit.Mqtt)
