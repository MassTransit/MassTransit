---

title: TransformConfigurationExtensions

---

# TransformConfigurationExtensions

Namespace: MassTransit

```csharp
public static class TransformConfigurationExtensions
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransformConfigurationExtensions](../masstransit/transformconfigurationextensions)

## Methods

### **UseTransform\<T\>(IConsumePipeConfigurator, Action\<ITransformConfigurator\<T\>\>)**

Apply a message transform, the behavior of which is defined inline using the configurator

```csharp
public static void UseTransform<T>(IConsumePipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>
The consume pipe configurator

`configure` [Action\<ITransformConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

### **UseTransform\<T\>(IConsumePipeConfigurator, Func\<ITransformSpecificationConfigurator\<T\>, IConsumeTransformSpecification\<T\>\>)**

Encapsulate the pipe behavior in a transaction

```csharp
public static void UseTransform<T>(IConsumePipeConfigurator configurator, Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IConsumePipeConfigurator](../../masstransit-abstractions/masstransit/iconsumepipeconfigurator)<br/>

`getSpecification` [Func\<ITransformSpecificationConfigurator\<T\>, IConsumeTransformSpecification\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseTransform\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Action\<ITransformConfigurator\<T\>\>)**

Apply a message transform, the behavior of which is defined inline using the configurator

```csharp
public static void UseTransform<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITransformConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>
The consume pipe configurator

`configure` [Action\<ITransformConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

### **UseTransform\<T\>(IPipeConfigurator\<ConsumeContext\<T\>\>, Func\<ITransformSpecificationConfigurator\<T\>, IConsumeTransformSpecification\<T\>\>)**

Encapsulate the pipe behavior in a transaction

```csharp
public static void UseTransform<T>(IPipeConfigurator<ConsumeContext<T>> configurator, Func<ITransformSpecificationConfigurator<T>, IConsumeTransformSpecification<T>> getSpecification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IPipeConfigurator\<ConsumeContext\<T\>\>](../../masstransit-abstractions/masstransit/ipipeconfigurator-1)<br/>

`getSpecification` [Func\<ITransformSpecificationConfigurator\<T\>, IConsumeTransformSpecification\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

### **UseTransform\<T\>(ISendPipeConfigurator, Action\<ITransformConfigurator\<T\>\>)**

Apply a transform on send to the message

```csharp
public static void UseTransform<T>(ISendPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [ISendPipeConfigurator](../../masstransit-abstractions/masstransit/isendpipeconfigurator)<br/>
The consume pipe configurator

`configure` [Action\<ITransformConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback

### **UseTransform\<T\>(IPublishPipeConfigurator, Action\<ITransformConfigurator\<T\>\>)**

Apply a transform on send to the message

```csharp
public static void UseTransform<T>(IPublishPipeConfigurator configurator, Action<ITransformConfigurator<T>> configure)
```

#### Type Parameters

`T`<br/>
The message type

#### Parameters

`configurator` [IPublishPipeConfigurator](../../masstransit-abstractions/masstransit/ipublishpipeconfigurator)<br/>
The consume pipe configurator

`configure` [Action\<ITransformConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
The configuration callback
