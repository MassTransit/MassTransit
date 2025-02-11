---

title: ConsumerPipeConfiguratorExtensions

---

# ConsumerPipeConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class ConsumerPipeConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerPipeConfiguratorExtensions](../masstransit/consumerpipeconfiguratorextensions)

## Methods

### **UseFilter\<TConsumer, T\>(IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, T\>\>, IFilter\<ConsumerConsumeContext\<TConsumer\>\>)**

Adds a filter to the pipe

```csharp
public static void UseFilter<TConsumer, T>(IPipeConfigurator<ConsumerConsumeContext<TConsumer, T>> configurator, IFilter<ConsumerConsumeContext<TConsumer>> filter)
```

#### Type Parameters

`TConsumer`<br/>

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<ConsumerConsumeContext\<TConsumer, T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filter` [IFilter\<ConsumerConsumeContext\<TConsumer\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>
The already built pipe
