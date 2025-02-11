---

title: ConsumerInterfaceType

---

# ConsumerInterfaceType

Namespace: MassTransit.Configuration

A standard asynchronous consumer message type, defined by IConsumer

```csharp
public class ConsumerInterfaceType : IMessageInterfaceType
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ConsumerInterfaceType](../masstransit-configuration/consumerinterfacetype)<br/>
Implements [IMessageInterfaceType](../masstransit-configuration/imessageinterfacetype)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **ConsumerInterfaceType(Type, Type)**

```csharp
public ConsumerInterfaceType(Type messageType, Type consumerType)
```

#### Parameters

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`consumerType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **GetConsumerConnector\<T\>()**

```csharp
public IConsumerMessageConnector<T> GetConsumerConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageConnector\<T\>](../masstransit-configuration/iconsumermessageconnector-1)<br/>

### **GetInstanceConnector\<T\>()**

```csharp
public IInstanceMessageConnector<T> GetInstanceConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInstanceMessageConnector\<T\>](../masstransit-configuration/iinstancemessageconnector-1)<br/>
