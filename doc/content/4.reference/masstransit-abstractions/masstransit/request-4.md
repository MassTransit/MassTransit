---

title: Request<TSaga, TRequest, TResponse, TResponse2>

---

# Request\<TSaga, TRequest, TResponse, TResponse2\>

Namespace: MassTransit

A request is a state-machine based request configuration that includes
 the events and states related to the execution of a request.

```csharp
public interface Request<TSaga, TRequest, TResponse, TResponse2> : Request<TSaga, TRequest, TResponse>
```

#### Type Parameters

`TSaga`<br/>

`TRequest`<br/>
The request type

`TResponse`<br/>
The response type

`TResponse2`<br/>

Implements [Request\<TSaga, TRequest, TResponse\>](../masstransit/request-3)

## Properties

### **Settings**

The settings that are used for the request, including the timeout

```csharp
public abstract RequestSettings<TSaga, TRequest, TResponse, TResponse2> Settings { get; }
```

#### Property Value

[RequestSettings\<TSaga, TRequest, TResponse, TResponse2\>](../masstransit/requestsettings-4)<br/>

### **Completed2**

The event that is raised when the request completes and the response is received

```csharp
public abstract Event<TResponse2> Completed2 { get; set; }
```

#### Property Value

[Event\<TResponse2\>](../masstransit/event-1)<br/>
