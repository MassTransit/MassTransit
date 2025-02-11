---

title: StartedActivity

---

# StartedActivity

Namespace: MassTransit.Logging

```csharp
public struct StartedActivity
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [StartedActivity](../masstransit-logging/startedactivity)

## Fields

### **Activity**

```csharp
public Activity Activity;
```

## Constructors

### **StartedActivity(Activity)**

```csharp
public StartedActivity(Activity activity)
```

#### Parameters

`activity` Activity<br/>

## Methods

### **SetTag(String, String)**

```csharp
public void SetTag(string key, string value)
```

#### Parameters

`key` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

`value` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br/>

### **Update\<T\>(SendContext\<T\>)**

```csharp
public void Update<T>(SendContext<T> context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` [SendContext\<T\>](../../masstransit-abstractions/masstransit/sendcontext-1)<br/>

### **AddExceptionEvent(Exception, Boolean)**

```csharp
public void AddExceptionEvent(Exception exception, bool escaped)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

`escaped` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Stop()**

```csharp
public void Stop()
```
