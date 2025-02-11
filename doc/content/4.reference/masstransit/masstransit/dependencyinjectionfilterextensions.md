---

title: DependencyInjectionFilterExtensions

---

# DependencyInjectionFilterExtensions

Namespace: MassTransit

```csharp
public static class DependencyInjectionFilterExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [DependencyInjectionFilterExtensions](../masstransit/dependencyinjectionfilterextensions)

## Methods

### **UseConsumeFilter(IConsumePipeConfigurator, Type, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseConsumeFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseConsumeFilter(IConsumePipeConfigurator, Type, IRegistrationContext, Action\<IMessageTypeFilterConfigurator\>)**

Use scoped filter for

```csharp
public static void UseConsumeFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context, Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

`configureMessageTypeFilter` [Action\<IMessageTypeFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Message type to which apply the filter

### **UseConsumeFilter\<TFilter\>(IConsumePipeConfigurator, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseConsumeFilter<TFilter>(IConsumePipeConfigurator configurator, IRegistrationContext context)
```

#### Type Parameters

`TFilter`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseSendFilter(ISendPipelineConfigurator, Type, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseSendFilter(ISendPipelineConfigurator configurator, Type filterType, IRegistrationContext context)
```

#### Parameters

`configurator` [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseSendFilter(ISendPipelineConfigurator, Type, IRegistrationContext, Action\<IMessageTypeFilterConfigurator\>)**

Use scoped filter for

```csharp
public static void UseSendFilter(ISendPipelineConfigurator configurator, Type filterType, IRegistrationContext context, Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
```

#### Parameters

`configurator` [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

`configureMessageTypeFilter` [Action\<IMessageTypeFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Message type to which apply the filter

### **UseSendFilter\<TFilter\>(ISendPipelineConfigurator, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseSendFilter<TFilter>(ISendPipelineConfigurator configurator, IRegistrationContext context)
```

#### Type Parameters

`TFilter`<br/>

#### Parameters

`configurator` [ISendPipelineConfigurator](../../masstransit-abstractions/masstransit/isendpipelineconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UsePublishFilter(IPublishPipelineConfigurator, Type, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UsePublishFilter(IPublishPipelineConfigurator configurator, Type filterType, IRegistrationContext context)
```

#### Parameters

`configurator` [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UsePublishFilter(IPublishPipelineConfigurator, Type, IRegistrationContext, Action\<IMessageTypeFilterConfigurator\>)**

Use scoped filter for

```csharp
public static void UsePublishFilter(IPublishPipelineConfigurator configurator, Type filterType, IRegistrationContext context, Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
```

#### Parameters

`configurator` [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

`configureMessageTypeFilter` [Action\<IMessageTypeFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Message type to which apply the filter

### **UsePublishFilter\<TFilter\>(IPublishPipelineConfigurator, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UsePublishFilter<TFilter>(IPublishPipelineConfigurator configurator, IRegistrationContext context)
```

#### Type Parameters

`TFilter`<br/>

#### Parameters

`configurator` [IPublishPipelineConfigurator](../../masstransit-abstractions/masstransit/ipublishpipelineconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseExecuteActivityFilter(IConsumePipeConfigurator, Type, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseExecuteActivityFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseExecuteActivityFilter(IConsumePipeConfigurator, Type, IRegistrationContext, Action\<IMessageTypeFilterConfigurator\>)**

Use scoped filter for

```csharp
public static void UseExecuteActivityFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context, Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

`configureMessageTypeFilter` [Action\<IMessageTypeFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Message type to which apply the filter

### **UseExecuteActivityFilter\<TFilter\>(IConsumePipeConfigurator, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseExecuteActivityFilter<TFilter>(IConsumePipeConfigurator configurator, IRegistrationContext context)
```

#### Type Parameters

`TFilter`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseCompensateActivityFilter(IConsumePipeConfigurator, Type, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseCompensateActivityFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

### **UseCompensateActivityFilter(IConsumePipeConfigurator, Type, IRegistrationContext, Action\<IMessageTypeFilterConfigurator\>)**

Use scoped filter for

```csharp
public static void UseCompensateActivityFilter(IConsumePipeConfigurator configurator, Type filterType, IRegistrationContext context, Action<IMessageTypeFilterConfigurator> configureMessageTypeFilter)
```

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>
Filter type

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context

`configureMessageTypeFilter` [Action\<IMessageTypeFilterConfigurator\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
Message type to which apply the filter

### **UseCompensateActivityFilter\<TFilter\>(IConsumePipeConfigurator, IRegistrationContext)**

Use scoped filter for

```csharp
public static void UseCompensateActivityFilter<TFilter>(IConsumePipeConfigurator configurator, IRegistrationContext context)
```

#### Type Parameters

`TFilter`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`context` [IRegistrationContext](../../masstransit-abstractions/masstransit/iregistrationcontext)<br/>
Configuration registration context
