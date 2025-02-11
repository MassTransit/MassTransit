---

title: PublishedMessageFilter

---

# PublishedMessageFilter

Namespace: MassTransit.Testing

```csharp
public class PublishedMessageFilter
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [PublishedMessageFilter](../masstransit-testing/publishedmessagefilter)

## Properties

### **Includes**

```csharp
public PublishedMessageFilterSet Includes { get; set; }
```

#### Property Value

[PublishedMessageFilterSet](../masstransit-testing/publishedmessagefilterset)<br/>

### **Excludes**

```csharp
public PublishedMessageFilterSet Excludes { get; set; }
```

#### Property Value

[PublishedMessageFilterSet](../masstransit-testing/publishedmessagefilterset)<br/>

## Constructors

### **PublishedMessageFilter()**

```csharp
public PublishedMessageFilter()
```

## Methods

### **Any(IPublishedMessage)**

```csharp
public bool Any(IPublishedMessage element)
```

#### Parameters

`element` [IPublishedMessage](../masstransit-testing/ipublishedmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **None(IPublishedMessage)**

```csharp
public bool None(IPublishedMessage element)
```

#### Parameters

`element` [IPublishedMessage](../masstransit-testing/ipublishedmessage)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
