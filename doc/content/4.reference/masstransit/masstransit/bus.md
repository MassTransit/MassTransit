---

title: Bus

---

# Bus

Namespace: MassTransit

used to get access to the bus factories

```csharp
public static class Bus
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Bus](../masstransit/bus)

## Properties

### **Factory**

Access a bus factory from this main factory interface (easy extension method support)

```csharp
public static IBusFactorySelector Factory { get; }
```

#### Property Value

[IBusFactorySelector](../masstransit/ibusfactoryselector)<br/>
