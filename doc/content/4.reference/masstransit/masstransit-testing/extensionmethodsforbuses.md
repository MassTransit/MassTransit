---

title: ExtensionMethodsForBuses

---

# ExtensionMethodsForBuses

Namespace: MassTransit.Testing

```csharp
public static class ExtensionMethodsForBuses
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ExtensionMethodsForBuses](../masstransit-testing/extensionmethodsforbuses)

## Methods

### **CreateBusActivityMonitor(IBus)**

#### Caution

Use the InactivityTask on the test harness instead

---

Creates a bus activity monitor

```csharp
public static IBusActivityMonitor CreateBusActivityMonitor(IBus bus)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

#### Returns

[IBusActivityMonitor](../masstransit-testing-implementations/ibusactivitymonitor)<br/>

### **CreateBusActivityMonitor(IBus, TimeSpan)**

#### Caution

Use the InactivityTask on the test harness instead

---

Creates a bus activity monitor

```csharp
public static IBusActivityMonitor CreateBusActivityMonitor(IBus bus, TimeSpan inactivityTimeout)
```

#### Parameters

`bus` [IBus](../../masstransit-abstractions/masstransit/ibus)<br/>

`inactivityTimeout` [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>
minimum time to wait to presume bus inactivity

#### Returns

[IBusActivityMonitor](../masstransit-testing-implementations/ibusactivitymonitor)<br/>
