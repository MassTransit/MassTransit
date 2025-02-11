---

title: RoutingSlipActivity

---

# RoutingSlipActivity

Namespace: MassTransit.Courier.Messages

```csharp
public class RoutingSlipActivity : Activity
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [RoutingSlipActivity](../masstransit-courier-messages/routingslipactivity)<br/>
Implements [Activity](../masstransit-courier-contracts/activity)

## Properties

### **Name**

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Address**

```csharp
public Uri Address { get; set; }
```

#### Property Value

Uri<br/>

### **Arguments**

```csharp
public IDictionary<string, object> Arguments { get; set; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

## Constructors

### **RoutingSlipActivity()**

```csharp
public RoutingSlipActivity()
```

### **RoutingSlipActivity(String, Uri, IDictionary\<String, Object\>)**

```csharp
public RoutingSlipActivity(string name, Uri address, IDictionary<string, object> arguments)
```

#### Parameters

`name` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`address` Uri<br/>

`arguments` [IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>

### **RoutingSlipActivity(Activity)**

```csharp
public RoutingSlipActivity(Activity activity)
```

#### Parameters

`activity` [Activity](../masstransit-courier-contracts/activity)<br/>
