---

title: IExceptionFilter

---

# IExceptionFilter

Namespace: MassTransit

Filter exceptions for policies that act based on an exception

```csharp
public interface IExceptionFilter : IProbeSite
```

Implements [IProbeSite](../masstransit/iprobesite)

## Methods

### **Match(Exception)**

Returns true if the exception matches the filter and the policy should
 be applied to the exception.

```csharp
bool Match(Exception exception)
```

#### Parameters

`exception` [Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>
The exception

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
True if the exception matches the filter, otherwise false.
