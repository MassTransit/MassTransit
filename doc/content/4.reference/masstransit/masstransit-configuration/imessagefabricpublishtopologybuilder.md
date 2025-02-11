---

title: IMessageFabricPublishTopologyBuilder

---

# IMessageFabricPublishTopologyBuilder

Namespace: MassTransit.Configuration

```csharp
public interface IMessageFabricPublishTopologyBuilder : IMessageFabricTopologyBuilder
```

Implements [IMessageFabricTopologyBuilder](../masstransit-configuration/imessagefabrictopologybuilder)

## Properties

### **ExchangeName**

```csharp
public abstract string ExchangeName { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **ExchangeType**

```csharp
public abstract ExchangeType ExchangeType { get; set; }
```

#### Property Value

[ExchangeType](../masstransit-transports-fabric/exchangetype)<br/>

## Methods

### **CreateImplementedBuilder()**

```csharp
IMessageFabricPublishTopologyBuilder CreateImplementedBuilder()
```

#### Returns

[IMessageFabricPublishTopologyBuilder](../masstransit-configuration/imessagefabricpublishtopologybuilder)<br/>
