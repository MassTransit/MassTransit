---

title: InterceptConfigurationExtensions

---

# InterceptConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InterceptConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InterceptConfigurationExtensions](../masstransit/interceptconfigurationextensions)

## Methods

### **UseIntercept\<T\>(IPipeConfigurator\<T\>, IPipe\<T\>)**

Adds a fork to the pipe, which invokes a separate pipe before passing to the next filter.

```csharp
public static void UseIntercept<T>(IPipeConfigurator<T> configurator, IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>
The filter to add
