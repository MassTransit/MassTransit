---

title: ICacheValue

---

# ICacheValue

Namespace: MassTransit.Internals.Caching

```csharp
public interface ICacheValue
```

## Properties

### **HasValue**

```csharp
public abstract bool HasValue { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **IsFaultedOrCanceled**

True if the node value is invalid

```csharp
public abstract bool IsFaultedOrCanceled { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Usage**

Tracks value usage

```csharp
public abstract int Usage { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Evict()**

Discard the value

```csharp
Task Evict()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
