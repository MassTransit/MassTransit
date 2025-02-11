---

title: IInactivityObservationSource

---

# IInactivityObservationSource

Namespace: MassTransit.Testing.Implementations

```csharp
public interface IInactivityObservationSource
```

## Properties

### **IsInactive**

True if the inactivity source is currently inactive

```csharp
public abstract bool IsInactive { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

## Methods

### **ConnectInactivityObserver(IInactivityObserver)**

```csharp
ConnectHandle ConnectInactivityObserver(IInactivityObserver observer)
```

#### Parameters

`observer` [IInactivityObserver](../masstransit-testing-implementations/iinactivityobserver)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
