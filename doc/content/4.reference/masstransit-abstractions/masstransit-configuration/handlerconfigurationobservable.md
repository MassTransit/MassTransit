---

title: HandlerConfigurationObservable

---

# HandlerConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class HandlerConfigurationObservable : Connectable<IHandlerConfigurationObserver>, IHandlerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IHandlerConfigurationObserver\>](../masstransit-util/connectable-1) → [HandlerConfigurationObservable](../masstransit-configuration/handlerconfigurationobservable)<br/>
Implements [IHandlerConfigurationObserver](../masstransit/ihandlerconfigurationobserver)

## Properties

### **Connected**

```csharp
public IHandlerConfigurationObserver[] Connected { get; }
```

#### Property Value

[IHandlerConfigurationObserver[]](../masstransit/ihandlerconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **HandlerConfigurationObservable()**

```csharp
public HandlerConfigurationObservable()
```

## Methods

### **HandlerConfigured\<TMessage\>(IHandlerConfigurator\<TMessage\>)**

```csharp
public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
```

#### Type Parameters

`TMessage`<br/>

#### Parameters

`configurator` [IHandlerConfigurator\<TMessage\>](../masstransit/ihandlerconfigurator-1)<br/>

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
