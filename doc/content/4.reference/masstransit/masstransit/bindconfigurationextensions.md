---

title: BindConfigurationExtensions

---

# BindConfigurationExtensions

Namespace: MassTransit

```csharp
public static class BindConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BindConfigurationExtensions](../masstransit/bindconfigurationextensions)

## Methods

### **UseBind\<TLeft\>(IPipeConfigurator\<TLeft\>, Action\<IBindConfigurator\<TLeft\>\>)**

Adds a filter to the pipe which is of a different type than the native pipe context type

```csharp
public static void UseBind<TLeft>(IPipeConfigurator<TLeft> configurator, Action<IBindConfigurator<TLeft>> configure)
```

#### Type Parameters

`TLeft`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<TLeft\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`configure` [Action\<IBindConfigurator\<TLeft\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
