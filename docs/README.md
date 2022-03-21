---
# home: true
layout: Wow
heroImage: /mt-logo-color.png
heroText: MassTransit
tagline: A free, open-source distributed application framework for .NET
actionText: Get Started →
actionLink: /getting-started/
transports:
- text: RabbitMQ
  link: /quick-starts/rabbitmq
  description: RabbitMQ is an Open Source project that provides a highly available 
- text: Azure Service Bus 
  link: /quick-starts/azure-service-bus
  description: Want to play in the Azure Cloud, use this transport to keep everything in Azure
- text: SQS
  link: /quick-starts/sqs
  description: Prefer the AWS cloud? Utilize the Serverless SNS + SQS model
features:
- title: Simple yet Sophisticated
  details: Easy to use and understand API allowing you to focus on solving business problems
- title: Transport Liquidity
  details: Deploy your solution using RabbitMQ, Azure Service Bus, ActiveMQ, and Amazon SQS/SNS without having to rewrite it
- title: Powerful Message Patterns
  details: Including message consumers, persistent sagas and event-driven state machines, and routing-slip based distributed transactions with compensation
- title: End-to-End Solution
  details: Handles message serialization, headers, broker topology, message routing, exceptions, retries, concurrency, connection and consumer lifecycle management
- title: Unit Testable
  details: In memory test harness for creating fast unit tests with comprehensive integration test level verification
- title: Monitoring
  details: Modern support for distributed tracing and service health and liveliness checks
footer: Apache 2.0 Licensed | Copyright © 2007-2022 Chris Patterson
---

MassTransit is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

MassTransit works with several well supported message transports and provides an [extensive set of developer-friendly features](usage/transports/) to build durable asynchronous services.
