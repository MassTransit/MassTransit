---

title: StartedInstrument

---

# StartedInstrument

Namespace: MassTransit.Logging

```csharp
public struct StartedInstrument
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [StartedInstrument](../masstransit-logging/startedinstrument)

## Constructors

### **StartedInstrument(Action\<Exception\>, Action)**

```csharp
public StartedInstrument(Action<Exception> onFault, Action onStop)
```

#### Parameters

`onFault` [Action\<Exception\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

`onStop` [Action](https://learn.microsoft.com/en-us/dotnet/api/system.action)<br/>

## Methods

### **AddException(Exception)**

```csharp
public void AddException(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **Stop()**

```csharp
public void Stop()
```
