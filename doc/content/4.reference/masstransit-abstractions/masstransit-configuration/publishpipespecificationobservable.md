---

title: PublishPipeSpecificationObservable

---

# PublishPipeSpecificationObservable

Namespace: MassTransit.Configuration

```csharp
public class PublishPipeSpecificationObservable : Connectable<IPublishPipeSpecificationObserver>, IPublishPipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IPublishPipeSpecificationObserver\>](../masstransit-util/connectable-1) → [PublishPipeSpecificationObservable](../masstransit-configuration/publishpipespecificationobservable)<br/>
Implements [IPublishPipeSpecificationObserver](../masstransit-configuration/ipublishpipespecificationobserver)

## Properties

### **Connected**

```csharp
public IPublishPipeSpecificationObserver[] Connected { get; }
```

#### Property Value

[IPublishPipeSpecificationObserver[]](../masstransit-configuration/ipublishpipespecificationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **PublishPipeSpecificationObservable()**

```csharp
public PublishPipeSpecificationObservable()
```

## Methods

### **MessageSpecificationCreated\<T\>(IMessagePublishPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessagePublishPipeSpecification\<T\>](../masstransit-configuration/imessagepublishpipespecification-1)<br/>

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
