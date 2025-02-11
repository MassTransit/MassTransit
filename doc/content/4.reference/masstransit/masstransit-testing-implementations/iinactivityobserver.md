---

title: IInactivityObserver

---

# IInactivityObserver

Namespace: MassTransit.Testing.Implementations

```csharp
public interface IInactivityObserver
```

## Methods

### **Connected(IInactivityObservationSource)**

```csharp
void Connected(IInactivityObservationSource source)
```

#### Parameters

`source` [IInactivityObservationSource](../masstransit-testing-implementations/iinactivityobservationsource)<br/>

### **NoActivity()**

```csharp
Task NoActivity()
```

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>

### **ForceInactive()**

```csharp
void ForceInactive()
```
