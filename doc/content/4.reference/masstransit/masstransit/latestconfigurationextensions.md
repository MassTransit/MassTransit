---

title: LatestConfigurationExtensions

---

# LatestConfigurationExtensions

Namespace: MassTransit

```csharp
public static class LatestConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [LatestConfigurationExtensions](../masstransit/latestconfigurationextensions)

## Methods

### **UseLatest\<T\>(IPipeConfigurator\<T\>, Action\<ILatestConfigurator\<T\>\>)**

Adds a latest value filter to the pipe

```csharp
public static void UseLatest<T>(IPipeConfigurator<T> configurator, Action<ILatestConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`configure` [Action\<ILatestConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
