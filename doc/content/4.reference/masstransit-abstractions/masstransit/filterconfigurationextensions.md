---

title: FilterConfigurationExtensions

---

# FilterConfigurationExtensions

Namespace: MassTransit

```csharp
public static class FilterConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [FilterConfigurationExtensions](../masstransit/filterconfigurationextensions)

## Methods

### **UseFilter\<T\>(IConsumePipeConfigurator, IFilter\<ConsumeContext\<T\>\>)**

Adds a filter to the consume pipe for the specific message type

```csharp
public static void UseFilter<T>(IConsumePipeConfigurator configurator, IFilter<ConsumeContext<T>> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IConsumePipeConfigurator](../masstransit/iconsumepipeconfigurator)<br/>
The pipe configurator

`filter` [IFilter\<ConsumeContext\<T\>\>](../masstransit/ifilter-1)<br/>
The filter to add

### **UseFilter\<T\>(ISendPipeConfigurator, IFilter\<SendContext\<T\>\>)**

Adds a filter to the send pipe for the specific message type

```csharp
public static void UseFilter<T>(ISendPipeConfigurator configurator, IFilter<SendContext<T>> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [ISendPipeConfigurator](../masstransit/isendpipeconfigurator)<br/>
The pipe configurator

`filter` [IFilter\<SendContext\<T\>\>](../masstransit/ifilter-1)<br/>
The filter to add

### **UseFilter\<T\>(IPublishPipeConfigurator, IFilter\<PublishContext\<T\>\>)**

Adds a filter to the publish pipe for the specific message type

```csharp
public static void UseFilter<T>(IPublishPipeConfigurator configurator, IFilter<PublishContext<T>> filter)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IPublishPipeConfigurator](../masstransit/ipublishpipeconfigurator)<br/>
The pipe configurator

`filter` [IFilter\<PublishContext\<T\>\>](../masstransit/ifilter-1)<br/>
The filter to add

### **UseFilter\<T\>(IPipeConfigurator\<T\>, IFilter\<T\>)**

Adds a filter to the pipe

```csharp
public static void UseFilter<T>(IPipeConfigurator<T> configurator, IFilter<T> filter)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filter` [IFilter\<T\>](../masstransit/ifilter-1)<br/>
The filter to add

### **UseFilters\<T\>(IPipeConfigurator\<T\>, IEnumerable\<IFilter\<T\>\>)**

Adds filters to the pipe

```csharp
public static void UseFilters<T>(IPipeConfigurator<T> configurator, IEnumerable<IFilter<T>> filters)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filters` [IEnumerable\<IFilter\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
The filters to add

### **UseFilters\<T\>(IPipeConfigurator\<T\>, IFilter`1[])**

Adds filters to the pipe

```csharp
public static void UseFilters<T>(IPipeConfigurator<T> configurator, IFilter`1[] filters)
```

#### Type Parameters

`T`<br/>
The context type

#### Parameters

`configurator` [IPipeConfigurator\<T\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filters` [IFilter`1[]](../masstransit/ifilter-1)<br/>
The filters to add

### **UseFilter\<TContext, TFilter\>(IPipeConfigurator\<TContext\>, IFilter\<TFilter\>, MergeFilterContextProvider\<TContext, TFilter\>, FilterContextProvider\<TFilter, TContext\>)**

Adds a filter to the pipe which is of a different type than the native pipe context type

```csharp
public static void UseFilter<TContext, TFilter>(IPipeConfigurator<TContext> configurator, IFilter<TFilter> filter, MergeFilterContextProvider<TContext, TFilter> contextProvider, FilterContextProvider<TFilter, TContext> inputContextProvider)
```

#### Type Parameters

`TContext`<br/>
The context type

`TFilter`<br/>
The filter context type

#### Parameters

`configurator` [IPipeConfigurator\<TContext\>](../masstransit/ipipeconfigurator-1)<br/>
The pipe configurator

`filter` [IFilter\<TFilter\>](../masstransit/ifilter-1)<br/>
The filter to add

`contextProvider` [MergeFilterContextProvider\<TContext, TFilter\>](../masstransit/mergefiltercontextprovider-2)<br/>

`inputContextProvider` [FilterContextProvider\<TFilter, TContext\>](../masstransit/filtercontextprovider-2)<br/>
