---

title: Activity

---

# Activity

Namespace: MassTransit.Courier.Contracts

```csharp
public interface Activity
```

## Properties

### **Name**

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Address**

```csharp
public abstract Uri Address { get; }
```

#### Property Value

Uri<br/>

### **Arguments**

```csharp
public abstract IDictionary<string, object> Arguments { get; }
```

#### Property Value

[IDictionary\<String, Object\>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br/>
