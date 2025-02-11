---

title: INotifyValueUsed

---

# INotifyValueUsed

Namespace: MassTransit.Caching

If a cached value implments this interface, the cache will attach itself to the
 event so the value can signal usage to update the lifetime of the value.

```csharp
public interface INotifyValueUsed
```

## Events

### **Used**

Should be raised by the value when used, to keep it alive in the cache.

```csharp
public abstract event Action Used;
```
