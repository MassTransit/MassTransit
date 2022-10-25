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
  description: RabbitMQ is a high performance, highly available freely available open source message broker
- text: Azure Service Bus 
  link: /quick-starts/azure-service-bus
  description: Want to play in the Azure Cloud, use this transport to keep everything in Azure
- text: SQS
  link: /quick-starts/sqs
  description: Prefer the AWS cloud? Utilize the Serverless SNS + SQS model
features:
- title: Simple yet Sophisticated
  details: Easy to use and understand, allowing you to focus on solving business problems
- title: Transport Liquidity
  details: Deploy using RabbitMQ, Azure Service Bus, ActiveMQ, and Amazon SQS/SNS without having to rewrite it
- title: Powerful Message Patterns
  details: Consumers, sagas, state machines, and choreography-based distributed transactions with compensation
- title: End-to-End Solution
  details: Handles message serialization, headers, and routing, broker topology, exceptions, retries, concurrency, connection and consumer lifecycle management
- title: Unit Testable
  details: Fast in-memory test harness to simplify unit testing, including sent, published, and consumed message observers
- title: Monitoring
  details: Modern support for distributed tracing, service health and liveliness checks
footer: Apache 2.0 Licensed | Copyright © 2007-2022 Chris Patterson
---

MassTransit is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

MassTransit works with several well supported message transports and provides an [extensive set of developer-friendly features](usage/transports/) to build durable asynchronous services.
