---

title: ConsumeTopologyConfigurationObservable

---

# ConsumeTopologyConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class ConsumeTopologyConfigurationObservable : Connectable<IConsumeTopologyConfigurationObserver>, IConsumeTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConsumeTopologyConfigurationObserver\>](../masstransit-util/connectable-1) → [ConsumeTopologyConfigurationObservable](../masstransit-configuration/consumetopologyconfigurationobservable)<br/>
Implements [IConsumeTopologyConfigurationObserver](../masstransit-configuration/iconsumetopologyconfigurationobserver)

## Properties

### **Connected**

```csharp
public IConsumeTopologyConfigurationObserver[] Connected { get; }
```

#### Property Value

[IConsumeTopologyConfigurationObserver[]](../masstransit-configuration/iconsumetopologyconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConsumeTopologyConfigurationObservable()**

```csharp
public ConsumeTopologyConfigurationObservable()
```

## Methods

### **MessageTopologyCreated\<T\>(IMessageConsumeTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> configuration)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configuration` [IMessageConsumeTopologyConfigurator\<T\>](../masstransit/imessageconsumetopologyconfigurator-1)<br/>

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
