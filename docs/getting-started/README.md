# Getting Started

Getting started with MassTransit is fast and easy.

## Pre-Requisites

- The [.NET 6 SDK](https://dotnet.microsoft.com/download) should be installed before continuing.
- Some examples use [Docker](https://www.docker.com/products/docker-desktop) to run backing services

## Select your transport:

- [In Memory](/quick-starts/in-memory): A dependency free way to get started, but not for production use
- [RabbitMQ](/quick-starts/rabbitmq): A high performance transport that allows both cloud based and local development
- [Azure Service Bus](/quick-starts/azure-service-bus): Use the power of Azure
- [SQS](/quick-starts/sqs): Use the power of AWS

## What are we going to build?

Each example goes through complete process of creating a messaging based system. We will configure the HostBuilder to use MassTransit, we will create a message, a consumer of that message.

Next up, the `AddMassTransit` extension is used to configure the bus in the container. The `UsingInMemory` (and `UsingRabbitMq`) method specifies the transport to use for the bus. Each transport has its own `UsingXxx` method.

### Quick note on terminology

A `Message` in MassTransit is just a Plain Old CLR Object or `POCO` for short. In MassTransit these can be a Class, Interface, or a Record. A Record is the currently recommended best practice.

A `Consumer` is a .Net class that implements `IConsumer<T>` and is some what similar to an ASP.Net Controller but with only a single action. These are registered using the `AddConsumer` method on the MassTransit Configuration Builder. The consumer is added as a Scoped Lifetime.

## Let's Get Started

If you aren't sure which transport you are going to want to use yet, we'd recommend trying the [in-memory](/quick-starts/in-memory)! It has no dependencies and can easily be upgraded to the others thanks to the MassTransit abstractions.