# Conductor

_Conductor_ is a messaging service mesh that extends MassTransit to include run-time service discovery, advanced message routing, and monitoring. Composed of a consistent set of capabilities, _Conductor_ decouples services and reduces infrastructure configuration, and keeps routing, fault handling, and monitoring separated. _Conductor_ can be configured using existing consumers, sagas, and activities, without modification, reducing adoption time and effort.

> _Conductor_ is a long journey, in fact, the code has been under thought and design for the past two years. Features will continue to be added, based upon demand and adoption, and it is early in the journey.

## Architecture

Conductor's core architectural principle is _extensibility_. As a modern architecture built upon the learning from decades of prior art, which includes everything from SOA, the ESB, and even the Atari 2600 game console, Conductor provides a set of atoms which can be composed into a variety of capabilities. These atoms provide the basic building blocks for creating distributed services.

Services that depend on other services are inheritently coupled to those services. Despite the marketecture about microservices being loosely-coupled, they are still coupled to their dependencies. And unlike a monolithic application which is coupled at design-time, development-time, and compile-time, microservices are coupled at run-time, which makes ensuring that all dependencies are available even more complicated than during earlier stages of development.

Conductor consists of several components, each of which has a specific function.

### Service Endpoint

MassTransit has three endpoint types.

- _Receive endpoints_ connect message consumers to the broker via a queue or subscription.
- _Send endpoints_ send messages to a specific destination such as a queue or an exchange on the broker.
- _Publish endpoints_ publish messages to connected receive endpoints, creating a copy of the message in each receive endpoints input queue.

Conductor adds a new endpoint type, the _Service Endpoint_. A service endpoint is a managed receive endpoint. As consumers, sagas, and activities are configured, the service endpoint captures the details: message types, consumer definitions, sent message and published events, states, activities, arguments and logs. These details are stored and advertised by the service endpoint so that they can be discovered at run-time.

A service endpoint uses a separate queue for control messages. By default, the _Control_ suffix is appended to the queue name in the matching name format. So, using the kebab-case endpoint name formatter, the control queue for `submit-order` would be `submit-order-control`. The alternative, which is highly recommended, is to use an instance-specific endpoint for control traffic that is unique to each _service instance_.

### Service Instance

A _service instance_ is a logical construct containing one or more service endpoints on the same bus instance. Each service instance has a temporary unique identifier (_Guid InstanceId_) that is generated every time the bus is started. When multiple instances of the same bus are started (scale out), each instance has its own unique identifier.

Each _service instance_ can have its own control queue, which is the preferred approach. Endpoint-specific control queues are shared by all service instances, so control messages are only consumed by one of the service instances. Instance-specific control queues allow each instance to receive all control traffic from the _service client_. Instance-specific control queues are named `Instance-<id>`, which in kebab-case would be `instance-abc123xyz`, and are temporary auto-delete queues.

### Service Client

The _service client_ is the client-side of _Conductor_, and extends the `IClientFactory` and `IRequestClient` interfaces. The service client _links_ to service endpoints at run-time to discover service details, including the address, message type, and other related information. The service client also keeps track of service endpoint capabilities, such as client-side partitioning and instance-specific endpoints.

When an application creates a request client, the request type is used to discover the service endpoint. A _Link_ event is published, and the response contains the service address for that message type along with the details for every known service instance (up or down). Service and instance details are retained until they expire or until the bus is stopped.

::: tip
A least one instance of a service endpoint must be started for the service client to discover the service address. Which in practice makes sense – an application will not receive a request response from a service endpoint that is offline.
:::
