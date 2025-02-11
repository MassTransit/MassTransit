---

title: ILatestFilter<T>

---

# ILatestFilter\<T\>

Namespace: MassTransit.Middleware

Maintains the latest context to be passed through the filter

```csharp
public interface ILatestFilter<T>
```

#### Type Parameters

`T`<br/>

## Properties

### **Latest**

The most recently completed context to pass through the filter

```csharp
public abstract Task<T> Latest { get; }
```

#### Property Value

[Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br/>
