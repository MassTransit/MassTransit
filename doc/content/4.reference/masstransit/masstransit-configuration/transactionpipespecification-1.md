---

title: TransactionPipeSpecification<T>

---

# TransactionPipeSpecification\<T\>

Namespace: MassTransit.Configuration

```csharp
public class TransactionPipeSpecification<T> : ITransactionConfigurator, IPipeSpecification<T>, ISpecification
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TransactionPipeSpecification\<T\>](../masstransit-configuration/transactionpipespecification-1)<br/>
Implements [ITransactionConfigurator](../masstransit/itransactionconfigurator), [IPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipespecification-1), [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Timeout**

```csharp
public TimeSpan Timeout { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **IsolationLevel**

```csharp
public IsolationLevel IsolationLevel { set; }
```

#### Property Value

IsolationLevel<br/>

## Constructors

### **TransactionPipeSpecification()**

```csharp
public TransactionPipeSpecification()
```

## Methods

### **Apply(IPipeBuilder\<T\>)**

```csharp
public void Apply(IPipeBuilder<T> builder)
```

#### Parameters

`builder` [IPipeBuilder\<T\>](../../masstransit-abstractions/masstransit-configuration/ipipebuilder-1)<br/>

### **Validate()**

```csharp
public IEnumerable<ValidationResult> Validate()
```

#### Returns

[IEnumerable\<ValidationResult\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br/>
