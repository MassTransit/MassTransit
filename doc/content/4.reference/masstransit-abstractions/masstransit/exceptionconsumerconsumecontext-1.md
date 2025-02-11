---

title: ExceptionConsumerConsumeContext<T>

---

# ExceptionConsumerConsumeContext\<T\>

Namespace: MassTransit

```csharp
public interface ExceptionConsumerConsumeContext<T> : ConsumerConsumeContext<T>, ConsumeContext, PipeContext, MessageContext, IPublishEndpoint, IPublishObserverConnector, ISendEndpointProvider, ISendObserverConnector
```

#### Type Parameters

`T`<br/>

Implements [ConsumerConsumeContext\<T\>](../masstransit/consumerconsumecontext-1), [ConsumeContext](../masstransit/consumecontext), [PipeContext](../masstransit/pipecontext), [MessageContext](../masstransit/messagecontext), [IPublishEndpoint](../masstransit/ipublishendpoint), [IPublishObserverConnector](../masstransit/ipublishobserverconnector), [ISendEndpointProvider](../masstransit/isendendpointprovider), [ISendObserverConnector](../masstransit/isendobserverconnector)

## Properties

### **Exception**

The exception that was thrown

```csharp
public abstract Exception Exception { get; }
```

#### Property Value

[Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception)<br/>

### **ExceptionInfo**

The exception info, suitable for inclusion in a fault message

```csharp
public abstract ExceptionInfo ExceptionInfo { get; }
```

#### Property Value

[ExceptionInfo](../masstransit/exceptioninfo)<br/>
