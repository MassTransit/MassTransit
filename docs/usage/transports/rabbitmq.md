# RabbitMQ configuration options

_This page has not been updated yet._

This is the recommended approach for configuring MassTransit for use with RabbitMQ.

```csharp
Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host(new Uri("rabbitmq://a-machine-name/a-virtual-host"), host =>
    {
        host.Username("username");
        host.Password("password");
    });
});
```