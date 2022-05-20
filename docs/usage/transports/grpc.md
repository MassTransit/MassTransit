# gRPC

> [MassTransit.Grpc](https://www.nuget.org/packages/MassTransit.Grpc) 

A new gRPC transport, designed to be a peer-to-peer distributed non-durable message transport, is now included. It's entirely in-memory, has zero dependencies, and allows multiple service instances to exchange messages across a shared message fabric.

::: warning
New, shiny, and very much in the early availability stage. There may be edge cases, so proceed with caution. The gRPC transport will be supported, and issues resolved as they're reported.
:::

[Introduction Video (YouTube)](https://youtu.be/ChtpCM3N5a8)

## Broker Topology

The gRPC is modeled after RabbitMQ, and supports many of the same features. It uses exchanges and queues, and follows the same topology structure as the RabbitMQ transport.

Fanout, Direct, and Topic exchanges are supported, along with routing key support.

And, it, is fast. Using the server GC, message throughput is pretty impressive.

On a single node (essentially in-memory, but serialized via protocol buffers):

```
Send: 253,774 msg/s
Consume: 172,996 msg/s
```

Across two nodes, load balanced via competing consumer:
```
Send: 232,597 msg/s
Consume: 36,331 msg/s
```

> Consume rate is slower because the messages are evenly split across the local and remote node.

## Examples

### Minimal

<<< @/docs/code/transports/GrpcConsoleListener.cs

Full documentation is coming soon, but for now the host configuration is shown below.

To configure the host using a complete address, such as `http://localhost:19796`, a `Uri` can be specified. The following configures a standalone instance, no servers are specified. Incoming connections are of course accepted.

```cs
cfg.Host(new Uri("http://localhost:19796"));
```

### Multiple Nodes

To configure a host that connects to other bus instances, use the _AddServer_ method in the host. In this example, the _host_ and _port_ are configured separately. The bus will not start until the server connections are established.

<<< @/docs/code/transports/GrpcMultiConsoleListener.cs

Check out [the discussion thread](https://github.com/MassTransit/MassTransit/discussions/2455) for more information.
