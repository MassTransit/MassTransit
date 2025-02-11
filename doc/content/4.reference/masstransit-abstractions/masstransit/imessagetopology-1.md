---

title: IMessageTopology<TMessage>

---

# IMessageTopology\<TMessage\>

Namespace: MassTransit

```csharp
public interface IMessageTopology<TMessage>
```

#### Type Parameters

`TMessage`<br/>

## Properties

### **EntityNameFormatter**

The entity name formatter for this message type

```csharp
public abstract IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; }
```

#### Property Value

[IMessageEntityNameFormatter\<TMessage\>](../masstransit/imessageentitynameformatter-1)<br/>

### **EntityName**

The formatted entity name for this message type

```csharp
public abstract string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
