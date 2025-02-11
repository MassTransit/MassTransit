---

title: ForkConfigurationExtensions

---

# ForkConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ForkConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ForkConfigurationExtensions](../masstransit/forkconfigurationextensions)

## Methods

### **UseFork\<T\>(IPipeConfigurator\<T\>, IPipe\<T\>)**

Adds a fork to the pipe, which invokes a separate pipe concurrently with the current pipe

```csharp
public static void UseFork<T>(IPipeConfigurator<T> configurator, IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
The filter to add
