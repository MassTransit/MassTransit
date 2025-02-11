---

title: BatchConsumerMessageConvention<T>

---

# BatchConsumerMessageConvention\<T\>

Namespace: MassTransit.Configuration

A convention that looks for IConsumerOfBatchOfT message types

```csharp
public class BatchConsumerMessageConvention<T> : IConsumerMessageConvention
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerMessageConvention\<T\>](../masstransit-configuration/batchconsumermessageconvention-1)<br/>
Implements [IConsumerMessageConvention](../masstransit-configuration/iconsumermessageconvention)

## Constructors

### **BatchConsumerMessageConvention()**

```csharp
public BatchConsumerMessageConvention()
```

## Methods

### **GetMessageTypes()**

```csharp
public IEnumerable<IMessageInterfaceType> GetMessageTypes()
```

#### Returns

[IEnumerable\<IMessageInterfaceType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
