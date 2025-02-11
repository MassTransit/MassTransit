---

title: IPendingValue<TValue>

---

# IPendingValue\<TValue\>

Namespace: MassTransit.Caching

A pending Get on an index, which has yet to be processed. Used by the
 node value factory to sequentially resolve the value for an index item
 which is then added to the cache.

```csharp
public interface IPendingValue<TValue>
```

#### Type Parameters

`TValue`<br/>
The value type

## Methods

### **SetValue(Task\<TValue\>)**

Sets the pending value, eliminating the need for the factory method.

```csharp
void SetValue(Task<TValue> value)
```

#### Parameters

`value` [Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
The resolved value

### **CreateValue()**

Create the value using the missing value factory supplied to Get

```csharp
Task<TValue> CreateValue()
```

#### Returns

[Task\<TValue\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
Either the value, or a faulted task.
