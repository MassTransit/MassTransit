# Guidance

The following recommendations should be considered _best practices_ for building applications using MassTransit, specifically with RabbitMQ.

- Published messages are routed to a receive endpoint queue by message type, using exchanges and exchange bindings. A service's receive endpoints do not affect other services or their receive endpoints, as long as they do not share the same queue. 
- Consumers and sagas should have their own receive endpoint, with a unique queue name
  - Each receive endpoint maps to one queue
  - A queue may contain more than one message type, the message type is used to deliver the message to the appropriate consumer configured on the receive endpoint.
  - If a received message is not handled by a consumer, the skipped message will be moved to a skipped queue, which is named with a \_skipped suffix.
- When running multiple instances of the same service
  - Use the same queue name for each instance
  - Messages from the queue will be load balanced across all instances (the _competing consumer_ pattern)
- If a consumer exception is thrown, the faulted message will be moved to an error queue, which is named with the \_error suffix.
- The number of concurrently processed messages can be up to the _PrefetchCount_, depending upon the number of cores available.
- For temporary receive endpoints that should be deleted when the bus is stopped, use _TemporaryEndpointDefinition_ as the receive endpoint definition.
- To configure _PrefetchCount_ higher than the desired concurrent message count, add _UseConcurrencyLimit(n)_ to the configuration. _This must be added before any consumers are configured._ Depending upon your consumer duration, higher values may greatly improve overall message throughput.





### Receive Endpoints

Consumers and sagas should be configured on their own receive endpoints. Running multiple unrelated consumers or sagas on a single receive endpoint is highly discouraged.

Running multiple instances of a service should use the same endpoint names for the same consumers and sagas to allow service instances to load balance messages from the same queue. 

Calling `ConfigureEndpoints` will generate a queue name for each receive endpoint based on the consumer, saga, or activity name (removing the _Consumer_, _Saga_, or _Activity_ suffix) using the specified _endpoint name formatter_ and configure the consumers and sagas using their respective endpoints.

In specialized scenarios where multiple consumers are closely related and have similar partitioning or ordering concerns, running those consumers on the same receive endpoint might be acceptable.