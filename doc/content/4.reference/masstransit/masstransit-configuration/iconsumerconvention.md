---

title: IConsumerConvention

---

# IConsumerConvention

Namespace: MassTransit.Configuration

A consumer convention is used to find message types inside a consumer class.

```csharp
public interface IConsumerConvention
```

## Methods

### **GetConsumerMessageConvention\<T\>()**

Returns the message convention for the type of T

```csharp
IConsumerMessageConvention GetConsumerMessageConvention<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageConvention](../masstransit-configuration/iconsumermessageconvention)<br/>
