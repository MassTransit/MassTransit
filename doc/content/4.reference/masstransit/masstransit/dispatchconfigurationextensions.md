---

title: DispatchConfigurationExtensions

---

# DispatchConfigurationExtensions

Namespace: MassTransit

```csharp
public static class DispatchConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DispatchConfigurationExtensions](../masstransit/dispatchconfigurationextensions)

## Methods

### **UseDispatch\<T\>(IPipeConfigurator\<T\>, IPipeContextConverterFactory\<T\>, Action\<IDispatchConfigurator\<T\>\>)**

Adds a dispatch filter to the pipe, which can be used to route traffic
 based on the type of the incoming context

```csharp
public static void UseDispatch<T>(IPipeConfigurator<T> configurator, IPipeContextConverterFactory<T> pipeContextProviderFactory, Action<IDispatchConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`pipeContextProviderFactory` [IPipeContextConverterFactory\<T\>](../masstransit-middleware/ipipecontextconverterfactory-1)<br/>

`configure` [Action\<IDispatchConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
