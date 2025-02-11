---

title: ITransactionConfigurator

---

# ITransactionConfigurator

Namespace: MassTransit

```csharp
public interface ITransactionConfigurator
```

## Properties

### **Timeout**

Sets the transaction timeout

```csharp
public abstract TimeSpan Timeout { set; }
```

#### Property Value

[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)<br/>

### **IsolationLevel**

Sets the isolation level of the transaction

```csharp
public abstract IsolationLevel IsolationLevel { set; }
```

#### Property Value

IsolationLevel<br/>
