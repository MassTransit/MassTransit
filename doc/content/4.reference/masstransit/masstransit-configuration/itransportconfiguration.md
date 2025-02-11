---

title: ITransportConfiguration

---

# ITransportConfiguration

Namespace: MassTransit.Configuration

```csharp
public interface ITransportConfiguration : ISpecification
```

Implements [ISpecification](../../masstransit-abstractions/masstransit/ispecification)

## Properties

### **Configurator**

```csharp
public abstract ITransportConfigurator Configurator { get; }
```

#### Property Value

[ITransportConfigurator](../../masstransit-abstractions/masstransit/itransportconfigurator)<br/>

### **PrefetchCount**

```csharp
public abstract int PrefetchCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **ConcurrentMessageLimit**

```csharp
public abstract Nullable<int> ConcurrentMessageLimit { get; }
```

#### Property Value

[Nullable\<Int32\>](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br/>

## Methods

### **GetConcurrentMessageLimit()**

```csharp
int GetConcurrentMessageLimit()
```

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>
