---

title: OutputPipeFilter<TInput, TOutput>

---

# OutputPipeFilter\<TInput, TOutput\>

Namespace: MassTransit.Middleware

Converts an inbound context type to a pipe context type post-dispatch

```csharp
public class OutputPipeFilter<TInput, TOutput> : IOutputPipeFilter<TInput, TOutput>, IFilter<TInput>, IProbeSite, IPipeConnector<TOutput>, IFilterObserverConnector<TOutput>
```

#### Type Parameters

`TInput`<br/>
The pipe context type

`TOutput`<br/>
The subsequent pipe context type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutputPipeFilter\<TInput, TOutput\>](../masstransit-middleware/outputpipefilter-2)<br/>
Implements [IOutputPipeFilter\<TInput, TOutput\>](../masstransit-middleware/ioutputpipefilter-2), [IFilter\<TInput\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TOutput\>](../masstransit-middleware/ipipeconnector-1), [IFilterObserverConnector\<TOutput\>](../../masstransit-abstractions/masstransit/ifilterobserverconnector-1)

## Constructors

### **OutputPipeFilter(IPipeContextConverter\<TInput, TOutput\>, FilterObservable, ITeeFilter\<TOutput\>)**

```csharp
public OutputPipeFilter(IPipeContextConverter<TInput, TOutput> contextConverter, FilterObservable observers, ITeeFilter<TOutput> outputFilter)
```

#### Parameters

`contextConverter` [IPipeContextConverter\<TInput, TOutput\>](../masstransit-middleware/ipipecontextconverter-2)<br/>

`observers` [FilterObservable](../../masstransit-abstractions/masstransit-observables/filterobservable)<br/>

`outputFilter` [ITeeFilter\<TOutput\>](../masstransit-middleware/iteefilter-1)<br/>
