# ActiveMQ

> [MassTransit.ActiveMQ](https://nuget.org/packages/MassTransit.ActiveMQ/)

## Topology

tbd

## Examples

### Minimal

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

## Amazon MQ

Amazon MQ uses ActiveMQ, so the same transport is used. Amazon MQ requires SSL, so if MassTransit detects the host name ends with `amazonaws.com`, SSL is automatically configured.

## Artemis

Artemis also supports the openwire protocol. However some differences exists that cause the Masstransit ActiveMQ transport provider not to function.
One of those causes is that Artemis works internally differentl with queues compared to ActiveMq. See [Artemis:Virtual Topics](https://activemq.apache.org/components/artemis/migration)

The easiest way to get the ActiveMQ transport provider working with a Artemis broker:

```cs
 var busControl = Bus.Factory.CreateUsingActiveMq(cfg =>
			{
				cfg.Host("localhost", 61618, cfgHost =>
				{
					cfgHost.Username("admin");
					cfgHost.Password("admin");
				});
				cfg.EnableArtemisCompatibility();
            });
```
Calling `cfg.EnableArtemisCompatibility()` will initialize the minimum necessary features so that the Masstransit ActiveMQ transport provider will work with the Artemis broker

Currently the only thing `cfg.EnableArtemisCompatibility()` does is setting a predefined formatter `ArtemisConsumerEndpointQueueNameFormatter` (which implements interface IActiveMqConsumerEndpointQueueNameFormatter) on the ConsumeTopology (accessible via cfg.ConsumeTopology )

Example of setting your own ConsumerEndpointQueueNameFormatter:
```
cfg.SetConsumerEndpointQueueNameFormatter(new MyCustomConsumerEndpointQueueNameFormatter());
```
So it is still possible to create your own IActiveMqConsumerEndpointQueueNameFormatter if you want to tweak the queue name.

The responsibility of the formatter is to create the queuename for a given 

    - a given receive/consumer endpoint name 
    - a given topic.


## TemporaryQueueNameFormatter

On the consume topology a TemporaryQueueNameFormatter can be configured. The responsibility of the formatter is to transform the 'system' generated name for a temporary queue.

This could be used to e.g. add a prefix to the generated temporary queuenames.
This helps to support namespaces in queuenames. 
Artemis can use this to enforce security policies

For adding a prefix, a handy helper is already provided.
During the configure lambda:

```cs
cfg.SetTemporaryQueueNamePrefix("mycustomnamespace.");
```

Behind the scenes this does something like this:

```cs
cfg.SetTemporaryQueueNameFormatter( new PrefixTemporaryQueueNameFormatter("mycustomnamespace."));
```
