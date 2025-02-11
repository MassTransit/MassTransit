---

title: MessageTopologyConfigurationObservable

---

# MessageTopologyConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class MessageTopologyConfigurationObservable : Connectable<IMessageTopologyConfigurationObserver>, IMessageTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IMessageTopologyConfigurationObserver\>](../masstransit-util/connectable-1) → [MessageTopologyConfigurationObservable](../masstransit-configuration/messagetopologyconfigurationobservable)<br/>
Implements [IMessageTopologyConfigurationObserver](../masstransit-configuration/imessagetopologyconfigurationobserver)

## Properties

### **Connected**

```csharp
public IMessageTopologyConfigurationObserver[] Connected { get; }
```

#### Property Value

[IMessageTopologyConfigurationObserver[]](../masstransit-configuration/imessagetopologyconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **MessageTopologyConfigurationObservable()**

```csharp
public MessageTopologyConfigurationObservable()
```

## Methods

### **MessageTopologyCreated\<T\>(IMessageTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configuration` [IMessageTopologyConfigurator\<T\>](../masstransit-configuration/imessagetopologyconfigurator-1)<br/>

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
