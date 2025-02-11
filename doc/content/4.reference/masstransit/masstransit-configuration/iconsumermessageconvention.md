---

title: IConsumerMessageConvention

---

# IConsumerMessageConvention

Namespace: MassTransit.Configuration

A convention that returns connectors for message types that are defined in the consumer
 type.

```csharp
public interface IConsumerMessageConvention
```

## Methods

### **GetMessageTypes()**

Returns the message types handled by the consumer class

```csharp
IEnumerable<IMessageInterfaceType> GetMessageTypes()
```

#### Returns

[IEnumerable\<IMessageInterfaceType\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
