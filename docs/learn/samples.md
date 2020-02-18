# Samples

Working code is an excellent way to learn how to use MassTransit features. The samples below show the capabilities of MassTransit, and can be cloned, forked, and explored to get a better understanding.

The new samples are standalone repositories, which use NuGet to pull dependencies exactly as a developerwould use MassTransit.

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

### Container

MassTransit supports several dependency injection containers. Examples of how to setup and use MassTransit with those containers is provided via a sample application.

> Only currently active containers are fully supported. Other containers are still supported, but considered legacy.

Features used:
- Autofac
- Castle Windsor
- StructureMap
- Lamar
- Simple Injector
- Microsoft Extensions Dependency Injection

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Containers)

### ASP.NET Core Web

Clone the sample: [GitHub Repository](https://github.com/phatboyg/Sample-DotNetCore-DI)


### ASP.NET Core Console

This sample uses the latest setup and integration to create a console service using .NET Core 2.2 (or later). This service can easily be deployed into a Docker container, and hosted in the Kubernetes cluster of choice.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-ConsoleService)

### Quartz

Features used:
- Scheduling
- Quartz

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Quartz)

### RabbitMQ Direct Exchange

Shows how to configure a consumer and a producer to use RabbitMQ direct exchange routing.

Features used:
- RabbitMQ

Clone the sample: [GitHub Repository](https://github.com/MassTransit/Sample-Quartz)

### Benchmark

Test the performance of MassTransit in your environment.

Clone the sample: [GitHub Repository](https://github.com/MassTransit/MassTransit-Benchmark)


### RabbitMQ MQTT Consumer

Utilise RabbitMQ as a MQTT server and consume IOT data.

Clone the sample: [GitHub Repository](https://github.com/morganphilo/MassTransit.Mqtt)
