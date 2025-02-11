---

title: IPipe<TContext>

---

# IPipe\<TContext\>

Namespace: MassTransit

```csharp
public interface IPipe<TContext> : IProbeSite
```

#### Type Parameters

`TContext`<br/>

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Send(TContext)**

The base primitive, Send delivers the pipe context of T to the pipe.

```csharp
Task Send(TContext context)
```

#### Parameters

`context` TContext<br/>
The pipe context of type T

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
A task which is completed once the pipe has processed the context
