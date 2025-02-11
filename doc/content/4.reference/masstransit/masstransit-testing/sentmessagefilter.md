---

title: SentMessageFilter

---

# SentMessageFilter

Namespace: MassTransit.Testing

```csharp
public class SentMessageFilter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [SentMessageFilter](../masstransit-testing/sentmessagefilter)

## Properties

### **Includes**

```csharp
public SentMessageFilterSet Includes { get; set; }
```

#### Property Value

[SentMessageFilterSet](../masstransit-testing/sentmessagefilterset)<br/>

### **Excludes**

```csharp
public SentMessageFilterSet Excludes { get; set; }
```

#### Property Value

[SentMessageFilterSet](../masstransit-testing/sentmessagefilterset)<br/>

## Constructors

### **SentMessageFilter()**

```csharp
public SentMessageFilter()
```

## Methods

### **Any(ISentMessage)**

```csharp
public bool Any(ISentMessage element)
```

#### Parameters

`element` [ISentMessage](../masstransit-testing/isentmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **None(ISentMessage)**

```csharp
public bool None(ISentMessage element)
```

#### Parameters

`element` [ISentMessage](../masstransit-testing/isentmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
