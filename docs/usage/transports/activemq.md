# ActiveMQ


```csharp
Bus.Factory.CreateUsingActiveMq(cfg =>
{
    cfg.Host("localhost", h =>
    {
        h.Username(TestUsername);
        h.Password(TestPassword);

        h.UseSsl();
    });
});
```

::: warning
When using ActiveMQ, receive endpoint queue names must _not_ include any `.` characters. Using a _dotted_ queue name will break pub/sub message routing. If using a dotted queue name is required, such as when interacting with an existing queue, disable topic binding.

```cs
endpoint.BindMessagesTopics = false;
```

Disabling topic binding on the receive endpoint is recommended so that invalid virtual consumer queues will not be created.
:::

## Amazon MQ

Amazon MQ uses ActiveMQ, so the same transport is used. There are some additional configuration concerns that must be addressed.

```csharp
Bus.Factory.CreateUsingActiveMq(cfg =>
{
    cfg.Host("{your-id}.mq.us-east-2.amazonaws.com", h =>
    {
        h.Username(TestUsername);
        h.Password(TestPassword);
    });
});
```

When the _amazonaws_ domain is present in the host, the configuration is automatically setup to be compatible with Amazon MQ (including SSL, etc.).
