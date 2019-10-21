# Using middleware

MassTransit is composed of a network of pipelines, which are used to dispatch messages from the transport, through the receive endpoint, past deserialization, and ultimately to the consumers. And these pipelines are entirely asynchronous, making them very fast and very flexible.

Middleware components can be added to every pipeline in MassTransit, allowing for complete customization of message processing. And the granular ways that middleware can be applied make it easy to focus a particular behavior into a single receive endpoint, a single consumer, a saga, or the entire bus.

Middleware components are configured via extension methods on any pipe configurator `IPipeConfigurator<T>`, and the extension methods all begin with `Use` to separate them from other methods.

* [Receive Pipeline](receive.md)

The details of many of the built-in middleware components follow.

* [Circuit breaker](circuit-breaker.md)
* [Concurrent message limit](concurrency-limit.md)
* [Rate limiter](rate-limiter.md)
* [Latest](latest.md)
* [Custom](custom.md)

