---

title: Recycle<T>

---

# Recycle\<T\>

Namespace: MassTransit.Util

Recycles a supervisor once it is stopped, replacing it with a new one

```csharp
public class Recycle<T>
```

#### Type Parameters

`T`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Recycle\<T\>](../masstransit-util/recycle-1)

## Properties

### **Supervisor**

```csharp
public T Supervisor { get; }
```

#### Property Value

T<br/>

## Constructors

### **Recycle(Func\<T\>)**

```csharp
public Recycle(Func<T> supervisorFactory)
```

#### Parameters

`supervisorFactory` [Func\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-1)<br/>
