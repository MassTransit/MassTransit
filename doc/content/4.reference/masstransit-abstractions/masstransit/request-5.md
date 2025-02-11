---

title: Request<TSaga, TRequest, TResponse, TResponse2, TResponse3>

---

# Request\<TSaga, TRequest, TResponse, TResponse2, TResponse3\>

Namespace: MassTransit

A request is a state-machine based request configuration that includes
 the events and states related to the execution of a request.

```csharp
public interface Request<TSaga, TRequest, TResponse, TResponse2, TResponse3> : Request<TSaga, TRequest, TResponse, TResponse2>, Request<TSaga, TRequest, TResponse>
```

#### Type Parameters

`TSaga`<br/>

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>

`TResponse3`<br/>

Implements [Request\<TSaga, TRequest, TResponse, TResponse2\>](../masstransit/request-4), [Request\<TSaga, TRequest, TResponse\>](../masstransit/request-3)

## Properties

### **Settings**

The settings that are used for the request, including the timeout

```csharp
public abstract RequestSettings<TSaga, TRequest, TResponse, TResponse2, TResponse3> Settings { get; }
```

#### Property Value

[RequestSettings\<TSaga, TRequest, TResponse, TResponse2, TResponse3\>](../masstransit/requestsettings-5)<br/>

### **Completed3**

The event that is raised when the request completes and the response is received

```csharp
public abstract Event<TResponse3> Completed3 { get; set; }
```

#### Property Value

[Event\<TResponse3\>](../masstransit/event-1)<br/>
