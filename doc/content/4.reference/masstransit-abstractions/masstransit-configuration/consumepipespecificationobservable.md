---

title: ConsumePipeSpecificationObservable

---

# ConsumePipeSpecificationObservable

Namespace: MassTransit.Configuration

```csharp
public class ConsumePipeSpecificationObservable : Connectable<IConsumePipeSpecificationObserver>, IConsumePipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConsumePipeSpecificationObserver\>](../masstransit-util/connectable-1) → [ConsumePipeSpecificationObservable](../masstransit-configuration/consumepipespecificationobservable)<br/>
Implements [IConsumePipeSpecificationObserver](../masstransit-configuration/iconsumepipespecificationobserver)

## Properties

### **Connected**

```csharp
public IConsumePipeSpecificationObserver[] Connected { get; }
```

#### Property Value

[IConsumePipeSpecificationObserver[]](../masstransit-configuration/iconsumepipespecificationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConsumePipeSpecificationObservable()**

```csharp
public ConsumePipeSpecificationObservable()
```

## Methods

### **MessageSpecificationCreated\<T\>(IMessageConsumePipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessageConsumePipeSpecification\<T\>](../masstransit-configuration/imessageconsumepipespecification-1)<br/>

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
