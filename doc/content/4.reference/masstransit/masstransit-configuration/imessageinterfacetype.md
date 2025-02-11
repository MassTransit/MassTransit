---

title: IMessageInterfaceType

---

# IMessageInterfaceType

Namespace: MassTransit.Configuration

```csharp
public interface IMessageInterfaceType
```

## Properties

### **MessageType**

```csharp
public abstract Type MessageType { get; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **GetConsumerConnector\<T\>()**

```csharp
IConsumerMessageConnector<T> GetConsumerConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IConsumerMessageConnector\<T\>](../masstransit-configuration/iconsumermessageconnector-1)<br/>

### **GetInstanceConnector\<T\>()**

```csharp
IInstanceMessageConnector<T> GetInstanceConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[IInstanceMessageConnector\<T\>](../masstransit-configuration/iinstancemessageconnector-1)<br/>
