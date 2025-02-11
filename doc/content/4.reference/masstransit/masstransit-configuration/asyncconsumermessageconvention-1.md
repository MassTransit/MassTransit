---

title: AsyncConsumerMessageConvention<T>

---

# AsyncConsumerMessageConvention\<T\>

Namespace: MassTransit.Configuration

A default convention that looks for IConsumerOfT message types

```csharp
public class AsyncConsumerMessageConvention<T> : IConsumerMessageConvention
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [AsyncConsumerMessageConvention\<T\>](../masstransit-configuration/asyncconsumermessageconvention-1)<br/>
Implements [IConsumerMessageConvention](../masstransit-configuration/iconsumermessageconvention)

## Constructors

### **AsyncConsumerMessageConvention()**

```csharp
public AsyncConsumerMessageConvention()
```

## Methods

### **GetMessageTypes()**

```csharp
public IEnumerable<IMessageInterfaceType> GetMessageTypes()
```

#### Returns

[IEnumerable\<IMessageInterfaceType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
