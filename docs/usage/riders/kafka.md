# Kafka

Kafka is supported as a [Rider](/usage/riders/), and supports consuming and producing messages from/to Kafka topics. The Confluent .NET client is used, and has been tested with the community edition (running in Docker).

### Topic Endpoints

> Uses [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/), [MassTransit.Kafka](https://nuget.org/packages/MassTransit.Kafka/), [MassTransit.Extensions.DependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

To consume a Kafka topic, configure a Rider within the bus configuration as shown.

<<< @/docs/code/riders/KafkaConsumer.cs

A _TopicEndpoint_ connects a Kafka Consumer to a topic, using the specified topic name. The consumer group specified should be unique to the application, and shared by a cluster of service instances for load balancing. Consumers and sagas can be configured on the topic endpoint, which should be registered in the rider configuration. While they configuration for topic endpoints is the same as a receive endpoint, there is no implicit binding of consumer message types to Kafka topics. The message type is specified on the TopicEndpoint as a generic argument.

### Producers

Producing messages to Kafka topics requires the producer to be registered. The producer can then be used to produce messages to the specified Kafka topic. In the example below, messages are produced to the Kafka topic as they are entered.

<<< @/docs/code/riders/KafkaProducer.cs


