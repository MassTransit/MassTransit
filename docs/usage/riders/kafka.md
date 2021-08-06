# Kafka

Kafka is supported as a [Rider](/usage/riders/), and supports consuming and producing messages from/to Kafka topics. The Confluent .NET client is used, and has been tested with the community edition (running in Docker).

### Topic Endpoints

> Uses [MassTransit.RabbitMQ](https://nuget.org/packages/MassTransit.RabbitMQ/), [MassTransit.Kafka](https://nuget.org/packages/MassTransit.Kafka/), [MassTransit.Extensions.DependencyInjection](https://www.nuget.org/packages/MassTransit.Extensions.DependencyInjection/)

> Note: the following examples are using the RabbitMQ Transport. You can also use InMemory Transport to achieve the same effect when developing. With that, there is no need to install MassTransit.RabbitMQ.
> `x.UsingInMemory((context,config) => config.ConfigureEndpoints(context));`


To consume a Kafka topic, configure a Rider within the bus configuration as shown.

<<< @/docs/code/riders/KafkaConsumer.cs

A _TopicEndpoint_ connects a Kafka Consumer to a topic, using the specified topic name. The consumer group specified should be unique to the application, and shared by a cluster of service instances for load balancing. Consumers and sagas can be configured on the topic endpoint, which should be registered in the rider configuration. While the configuration for topic endpoints is the same as a receive endpoint, there is no implicit binding of consumer message types to Kafka topics. The message type is specified on the TopicEndpoint as a generic argument.

#### Configure Topology

When client has *required* permissions and `CreateIfMissing` is configured, topic can be created on startup and deleted on shutdown 

<<< @/docs/code/riders/KafkaTopicTopology.cs

### Producers

Producing messages to Kafka topics requires the producer to be registered. The producer can then be used to produce messages to the specified Kafka topic. In the example below, messages are produced to the Kafka topic as they are entered by the user.

<<< @/docs/code/riders/KafkaProducer.cs

### Producing and Consuming Multiple Message Types on a Single Topic

There are situations where you might want to produce / consume events of different types on the same Kafka topic. A common use case is to use a single topic to log ordered meaningful state change events like `SomethingRequested`, `SomethingStarted`, `SomethingFinished`.

Confluent have some documentation about how this can be implemented on the Schema Registry side:

- [Confluent Docs - Multiple Event Types in the Same Topic](https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#multiple-event-types-in-the-same-topic)
- [Confluent Docs - Multiple Event Types in the Same Topic with Avro](https://docs.confluent.io/platform/current/schema-registry/serdes-develop/serdes-avro.html#multiple-event-types-in-the-same-topic)
- [Confluent Blog - Multiple Event Types in the Same Topic](https://www.confluent.io/blog/multiple-event-types-in-the-same-kafka-topic/)

Unfortunately, it is [not yet widely supported in client tools and products](https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#limitations) and there is limited documentation about how to support this in your own applications. 

However, it is possible... The following demo uses the MassTransit Kafka Rider with custom [Avro](https://avro.apache.org/docs/current/) serializer / deserializer implementations and the Schema Registry to support multiple event types on a single topic:

[MassTransit-Kafka-Demo](https://github.com/danmalcolm/masstransit-kafka-demo)

The custom serializers / deserializer implementations leverage the wire format used by the standard Confluent schema-based serializers, which includes the schema id in the data stored for each message. This is also good news for interoperability with non-MassTransit applications.

**Warning: It's a little hacky and only supports the Avro format, but there's enough there to get you started.**
