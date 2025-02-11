---

title: StringInlineMessageData

---

# StringInlineMessageData

Namespace: MassTransit.MessageData.Values

```csharp
public class StringInlineMessageData : MessageData<String>, IMessageData, IInlineMessageData
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [StringInlineMessageData](../masstransit-messagedata-values/stringinlinemessagedata)<br/>
Implements [MessageData\<String\>](../../masstransit-abstractions/masstransit/messagedata-1), [IMessageData](../../masstransit-abstractions/masstransit/imessagedata), [IInlineMessageData](../masstransit-messagedata/iinlinemessagedata)

## Properties

### **Address**

```csharp
public Uri Address { get; }
```

#### Property Value

Uri<br/>

### **HasValue**

```csharp
public bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Value**

```csharp
public Task<string> Value { get; }
```

#### Property Value

[Task\<String\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>

## Constructors

### **StringInlineMessageData(String, Uri)**

```csharp
public StringInlineMessageData(string value, Uri address)
```

#### Parameters

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`address` Uri<br/>

## Methods

### **Set(IMessageDataReference)**

```csharp
public void Set(IMessageDataReference reference)
```

#### Parameters

`reference` [IMessageDataReference](../masstransit-messagedata/imessagedatareference)<br/>
