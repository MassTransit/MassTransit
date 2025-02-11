---

title: ITransportConfigurator

---

# ITransportConfigurator

Namespace: MassTransit

```csharp
public interface ITransportConfigurator
```

## Properties

### **PrefetchCount**

```csharp
public abstract int PrefetchCount { set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { set; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>
