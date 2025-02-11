---

title: ReceivedMessageFilter

---

# ReceivedMessageFilter

Namespace: MassTransit.Testing

```csharp
public class ReceivedMessageFilter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ReceivedMessageFilter](../masstransit-testing/receivedmessagefilter)

## Properties

### **Includes**

```csharp
public ReceivedMessageFilterSet Includes { get; set; }
```

#### Property Value

[ReceivedMessageFilterSet](../masstransit-testing/receivedmessagefilterset)<br/>

### **Excludes**

```csharp
public ReceivedMessageFilterSet Excludes { get; set; }
```

#### Property Value

[ReceivedMessageFilterSet](../masstransit-testing/receivedmessagefilterset)<br/>

## Constructors

### **ReceivedMessageFilter()**

```csharp
public ReceivedMessageFilter()
```

## Methods

### **Any(IReceivedMessage)**

```csharp
public bool Any(IReceivedMessage element)
```

#### Parameters

`element` [IReceivedMessage](../masstransit-testing/ireceivedmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **None(IReceivedMessage)**

```csharp
public bool None(IReceivedMessage element)
```

#### Parameters

`element` [IReceivedMessage](../masstransit-testing/ireceivedmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
