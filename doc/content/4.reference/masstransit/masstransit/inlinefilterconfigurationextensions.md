---

title: InlineFilterConfigurationExtensions

---

# InlineFilterConfigurationExtensions

Namespace: MassTransit

```csharp
public static class InlineFilterConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [InlineFilterConfigurationExtensions](../masstransit/inlinefilterconfigurationextensions)

## Methods

### **UseInlineFilter\<T\>(IPipeConfigurator\<T\>, InlineFilterMethod\<T\>)**

Creates an inline filter using a simple async method

```csharp
public static void UseInlineFilter<T>(IPipeConfigurator<T> configurator, InlineFilterMethod<T> inlineFilterMethod)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`inlineFilterMethod` [InlineFilterMethod\<T\>](../masstransit/inlinefiltermethod-1)<br/>
The inline filter delegate
