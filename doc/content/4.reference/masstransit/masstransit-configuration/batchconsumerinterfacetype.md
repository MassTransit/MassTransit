---

title: BatchConsumerInterfaceType

---

# BatchConsumerInterfaceType

Namespace: MassTransit.Configuration

A batch consumer

```csharp
public class BatchConsumerInterfaceType : IMessageInterfaceType
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchConsumerInterfaceType](../masstransit-configuration/batchconsumerinterfacetype)<br/>
Implements [IMessageInterfaceType](../masstransit-configuration/imessageinterfacetype)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **BatchConsumerInterfaceType(Type, Type, Type)**

```csharp
public BatchConsumerInterfaceType(Type batchMessageType, Type messageType, Type consumerType)
```

#### Parameters

`batchMessageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

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
