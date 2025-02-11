---

title: SagaInterfaceType

---

# SagaInterfaceType

Namespace: MassTransit.Configuration

```csharp
public class SagaInterfaceType
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SagaInterfaceType](../masstransit-configuration/sagainterfacetype)

## Properties

### **InterfaceType**

```csharp
public Type InterfaceType { get; private set; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

### **MessageType**

```csharp
public Type MessageType { get; private set; }
```

#### Property Value

[Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Constructors

### **SagaInterfaceType(Type, Type, Type)**

```csharp
public SagaInterfaceType(Type interfaceType, Type messageType, Type sagaType)
```

#### Parameters

`interfaceType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`messageType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`sagaType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

## Methods

### **GetInitiatedByConnector\<T\>()**

```csharp
public ISagaMessageConnector<T> GetInitiatedByConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageConnector\<T\>](../masstransit-configuration/isagamessageconnector-1)<br/>

### **GetOrchestratesConnector\<T\>()**

```csharp
public ISagaMessageConnector<T> GetOrchestratesConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageConnector\<T\>](../masstransit-configuration/isagamessageconnector-1)<br/>

### **GetObservesConnector\<T\>()**

```csharp
public ISagaMessageConnector<T> GetObservesConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageConnector\<T\>](../masstransit-configuration/isagamessageconnector-1)<br/>

### **GetInitiatedByOrOrchestratesConnector\<T\>()**

```csharp
public ISagaMessageConnector<T> GetInitiatedByOrOrchestratesConnector<T>()
```

#### Type Parameters

`T`<br/>

#### Returns

[ISagaMessageConnector\<T\>](../masstransit-configuration/isagamessageconnector-1)<br/>
