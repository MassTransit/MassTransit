---

title: SagaPipeConfiguratorExtensions

---

# SagaPipeConfiguratorExtensions

Namespace: MassTransit

```csharp
public static class SagaPipeConfiguratorExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaPipeConfiguratorExtensions](../masstransit/sagapipeconfiguratorextensions)

## Methods

### **UseFilter\<TSaga, T\>(IPipeConfigurator\<SagaConsumeContext\<TSaga, T\>\>, IFilter\<SagaConsumeContext\<TSaga\>\>)**

Adds a filter to the pipe

```csharp
public static void UseFilter<TSaga, T>(IPipeConfigurator<SagaConsumeContext<TSaga, T>> configurator, IFilter<SagaConsumeContext<TSaga>> filter)
```

#### Type Parameters

`TSaga`<br/>

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<SagaConsumeContext\<TSaga, T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filter` [IFilter\<SagaConsumeContext\<TSaga\>\>](../../masstransit-abstractions/masstransit/ifilter-1)<br/>
The already built pipe
