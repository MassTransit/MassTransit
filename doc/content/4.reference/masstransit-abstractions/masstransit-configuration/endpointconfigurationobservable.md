---

title: EndpointConfigurationObservable

---

# EndpointConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class EndpointConfigurationObservable : Connectable<IEndpointConfigurationObserver>, IEndpointConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IEndpointConfigurationObserver\>](../masstransit-util/connectable-1) → [EndpointConfigurationObservable](../masstransit-configuration/endpointconfigurationobservable)<br/>
Implements [IEndpointConfigurationObserver](../masstransit/iendpointconfigurationobserver)

## Properties

### **Connected**

```csharp
public IEndpointConfigurationObserver[] Connected { get; }
```

#### Property Value

[IEndpointConfigurationObserver[]](../masstransit/iendpointconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **EndpointConfigurationObservable()**

```csharp
public EndpointConfigurationObservable()
```

## Methods

### **EndpointConfigured\<T\>(T)**

```csharp
public void EndpointConfigured<T>(T configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` T<br/>

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
