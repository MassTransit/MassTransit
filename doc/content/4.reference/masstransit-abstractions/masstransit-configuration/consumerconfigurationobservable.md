---

title: ConsumerConfigurationObservable

---

# ConsumerConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class ConsumerConfigurationObservable : Connectable<IConsumerConfigurationObserver>, IConsumerConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IConsumerConfigurationObserver\>](../masstransit-util/connectable-1) → [ConsumerConfigurationObservable](../masstransit-configuration/consumerconfigurationobservable)<br/>
Implements [IConsumerConfigurationObserver](../masstransit/iconsumerconfigurationobserver)

## Properties

### **Connected**

```csharp
public IConsumerConfigurationObserver[] Connected { get; }
```

#### Property Value

[IConsumerConfigurationObserver[]](../masstransit/iconsumerconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **ConsumerConfigurationObservable()**

```csharp
public ConsumerConfigurationObservable()
```

## Methods

### **ConsumerConfigured\<TConsumer\>(IConsumerConfigurator\<TConsumer\>)**

```csharp
public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
```

#### Type Parameters

`TConsumer`<br/>

#### Parameters

`configurator` [IConsumerConfigurator\<TConsumer\>](../masstransit/iconsumerconfigurator-1)<br/>

### **ConsumerMessageConfigured\<TConsumer, TMessage\>(IConsumerMessageConfigurator\<TConsumer, TMessage\>)**

```csharp
public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
```

#### Type Parameters

`TConsumer`<br/>

`TMessage`<br/>

#### Parameters

`configurator` [IConsumerMessageConfigurator\<TConsumer, TMessage\>](../masstransit/iconsumermessageconfigurator-2)<br/>

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
