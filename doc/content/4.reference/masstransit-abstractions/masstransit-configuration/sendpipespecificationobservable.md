---

title: SendPipeSpecificationObservable

---

# SendPipeSpecificationObservable

Namespace: MassTransit.Configuration

```csharp
public class SendPipeSpecificationObservable : Connectable<ISendPipeSpecificationObserver>, ISendPipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<ISendPipeSpecificationObserver\>](../masstransit-util/connectable-1) → [SendPipeSpecificationObservable](../masstransit-configuration/sendpipespecificationobservable)<br/>
Implements [ISendPipeSpecificationObserver](../masstransit-configuration/isendpipespecificationobserver)

## Properties

### **Connected**

```csharp
public ISendPipeSpecificationObserver[] Connected { get; }
```

#### Property Value

[ISendPipeSpecificationObserver[]](../masstransit-configuration/isendpipespecificationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **SendPipeSpecificationObservable()**

```csharp
public SendPipeSpecificationObservable()
```

## Methods

### **MessageSpecificationCreated\<T\>(IMessageSendPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessageSendPipeSpecification\<T\>](../masstransit-configuration/imessagesendpipespecification-1)<br/>

### **Method4()**

```csharp
public void Method4()
```

### **Method5()**

```csharp
public void Method5()
```

### **Method6()**

```csharp
public void Method6()
```
