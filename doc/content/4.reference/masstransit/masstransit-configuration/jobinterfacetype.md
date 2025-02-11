---

title: JobInterfaceType

---

# JobInterfaceType

Namespace: MassTransit.Configuration

A job consumer

```csharp
public class JobInterfaceType : IMessageInterfaceType
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [JobInterfaceType](../masstransit-configuration/jobinterfacetype)<br/>
Implements [IMessageInterfaceType](../masstransit-configuration/imessageinterfacetype)

## Properties

### **MessageType**

```csharp
public Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **JobInterfaceType(Type, Type)**

```csharp
public JobInterfaceType(Type messageType, Type consumerType)
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
