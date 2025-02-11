---

title: PublishTopologyConfigurationObservable

---

# PublishTopologyConfigurationObservable

Namespace: MassTransit.Configuration

```csharp
public class PublishTopologyConfigurationObservable : Connectable<IPublishTopologyConfigurationObserver>, IPublishTopologyConfigurationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<IPublishTopologyConfigurationObserver\>](../masstransit-util/connectable-1) → [PublishTopologyConfigurationObservable](../masstransit-configuration/publishtopologyconfigurationobservable)<br/>
Implements [IPublishTopologyConfigurationObserver](../masstransit-configuration/ipublishtopologyconfigurationobserver)

## Properties

### **Connected**

```csharp
public IPublishTopologyConfigurationObserver[] Connected { get; }
```

#### Property Value

[IPublishTopologyConfigurationObserver[]](../masstransit-configuration/ipublishtopologyconfigurationobserver)<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **PublishTopologyConfigurationObservable()**

```csharp
public PublishTopologyConfigurationObservable()
```

## Methods

### **MessageTopologyCreated\<T\>(IMessagePublishTopologyConfigurator\<T\>)**

```csharp
public void MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurator` [IMessagePublishTopologyConfigurator\<T\>](../masstransit/imessagepublishtopologyconfigurator-1)<br/>

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
