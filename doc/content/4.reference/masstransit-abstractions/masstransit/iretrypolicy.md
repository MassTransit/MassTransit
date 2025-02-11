---

title: IRetryPolicy

---

# IRetryPolicy

Namespace: MassTransit

A retry policy determines how exceptions are handled, and whether or not the
 remaining filters should be retried

```csharp
public interface IRetryPolicy : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **CreatePolicyContext\<T\>(T)**

Creates a retry policy context for the retry, which initiates the exception tracking

```csharp
RetryPolicyContext<T> CreatePolicyContext<T>(T context)
```

#### Type Parameters

`T`<br/>

#### Parameters

`context` T<br/>

#### Returns

[RetryPolicyContext\<T\>](../masstransit/retrypolicycontext-1)<br/>

### **IsHandled(Exception)**

If the retry policy handles the exception, should return true

```csharp
bool IsHandled(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
