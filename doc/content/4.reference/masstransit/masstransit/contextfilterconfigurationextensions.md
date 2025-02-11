---

title: ContextFilterConfigurationExtensions

---

# ContextFilterConfigurationExtensions

Namespace: MassTransit

```csharp
public static class ContextFilterConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ContextFilterConfigurationExtensions](../masstransit/contextfilterconfigurationextensions)

## Methods

### **UseContextFilter\<T\>(IPipeConfigurator\<T\>, Func\<T, Task\<Boolean\>\>)**

Adds a content filter that uses a delegate to filter the context and only accept messages
 which pass the filter specification.

```csharp
public static void UseContextFilter<T>(IPipeConfigurator<T> configurator, Func<T, Task<bool>> filter)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`filter` [Func\<T, Task\<Boolean\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
A filter method that returns true to accept the message, or false to discard it
