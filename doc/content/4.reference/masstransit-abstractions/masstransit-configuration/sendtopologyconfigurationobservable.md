---

title: SendTopologyConfigurationObservable

---

# SendTopologyConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class SendTopologyConfigurationObservable : Connectable<ISendTopologyConfigurationObserver>, ISendTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<ISendTopologyConfigurationObserver\>](../masstransit-util/connectable-1) → [SendTopologyConfigurationObservable](../masstransit-configuration/sendtopologyconfigurationobservable)<br/>
Implements [ISendTopologyConfigurationObserver](../masstransit-configuration/isendtopologyconfigurationobserver)

## Properties

### **Connected**

```csharp
public ISendTopologyConfigurationObserver[] Connected { get; }
```

#### Property Value

[ISendTopologyConfigurationObserver[]](../masstransit-configuration/isendtopologyconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **SendTopologyConfigurationObservable()**

```csharp
public SendTopologyConfigurationObservable()
```

## Methods

### **MessageTopologyCreated\<T\>(IMessageSendTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> configuration)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configuration` [IMessageSendTopologyConfigurator\<T\>](../masstransit/imessagesendtopologyconfigurator-1)<br/>

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
