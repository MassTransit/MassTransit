---

title: FaultEntityNameAttribute

---

# FaultEntityNameAttribute

Namespace: MassTransit

Specify the EntityName used for the Fault version of this message contract, overriding the configured [IEntityNameFormatter](../masstransit/ientitynameformatter)
 if configured.

```csharp
public class FaultEntityNameAttribute : Attribute
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute) → [FaultEntityNameAttribute](../masstransit/faultentitynameattribute)

## Properties

### **EntityName**

```csharp
public string EntityName { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **TypeId**

```csharp
public object TypeId { get; }
```

#### Property Value

[Object](https://learn.microsoft.com/en-us/dotnet/api/system.object)<br/>

## Constructors

### **FaultEntityNameAttribute(String)**



```csharp
public FaultEntityNameAttribute(string entityName)
```

#### Parameters

`entityName` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>
The entity name to use for the faulted message type
