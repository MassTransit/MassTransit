# ActiveMQ

> [MassTransit.ActiveMQ](https://nuget.org/packages/MassTransit.ActiveMQ/)

In the example below, the ActiveMQ settings are configured.

<<< @/docs/code/transports/ActiveMqConsoleListener.cs

The configuration includes:

* The ActiveMQ host
  - Host name: `localhost`
  - User name and password used to connect to the host

The port can also be specified as an additional parameter on the _Host_ method. If port 61617 is specified, SSL is automatically enabled.

MassTransit includes several receive endpoint level configuration options that control receive endpoint behavior.

| Property                | Type   | Description 
|-------------------------|--------|------------------
| PrefetchCount         | ushort | The number of unacknowledged messages that can be processed concurrently (default based on CPU count)
| AutoDelete         | bool | If true, the queue will be automatically deleted when the bus is stopped (default: false)
| Durable        | bool   | If true, messages are persisted to disk before being acknowledged (default: true)

::: warning
When using ActiveMQ, receive endpoint queue names must _not_ include any `.` characters. Using a _dotted_ queue name will break pub/sub message routing. If using a dotted queue name is required, such as when interacting with an existing queue, disable topic binding.

```cs
endpoint.ConfigureConsumeTopology = false;
```

When the consume topology is not configured, the virtual consumer queues are not created.
:::

### Amazon MQ

Amazon MQ uses ActiveMQ, so the same transport is used. Amazon MQ requires SSL, so if MassTransit detects the host name ends with `amazonaws.com`, SSL is automatically configured.
