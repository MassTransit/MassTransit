# Topology

In MassTransit, _Topology_ is how message types are used to configure broker topics (exchanges in RabbitMQ) and queues. Topology is also used to access specific broker capabilities, such as RabbitMQ direct exchanges and routing keys.

Topology is separate from the send, publish, and consume pipelines which are focused more on middleware inside MassTransit. Topology allows conventions to be created that can create message-specific topology configuration at runtime as messages are published and sent.

### Bus Topology

Once the bus is created, access to topology is via the _Topology_ property on _IBus_. The _IBusTopology_ interface is shown below.

<<< @/src/MassTransit.Abstractions/Topology/IBusTopology.cs

The message, publish, and send topologies can be accessed using this interface. It is also possible to retrieve a message's publish address. The _Topology_ property may support other interfaces, such as a transport-specific host topology. Pattern matching can be used to check the host topology type as shown below.

<<< @/docs/code/advanced/BusHostTopologyMatch.cs

