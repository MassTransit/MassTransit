---

title: TimestampVariable

---

# TimestampVariable

Namespace: MassTransit.Initializers.Variables

Used to set timestamp(s) in a message, which is the same regardless of how many times it is
 used within the same initialize message context

```csharp
public class TimestampVariable : IInitializerVariable<DateTime>
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [TimestampVariable](../masstransit-initializers-variables/timestampvariable)<br/>
Implements [IInitializerVariable\<DateTime\>](../masstransit-initializers/iinitializervariable-1)

## Constructors

### **TimestampVariable()**

```csharp
public TimestampVariable()
```

### **TimestampVariable(DateTime)**

```csharp
public TimestampVariable(DateTime timestamp)
```

#### Parameters

`timestamp` [DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime)<br/>
