---

title: OutputPipeFilter<TInput, TOutput, TKey>

---

# OutputPipeFilter\<TInput, TOutput, TKey\>

Namespace: MassTransit.Middleware

```csharp
public class OutputPipeFilter<TInput, TOutput, TKey> : OutputPipeFilter<TInput, TOutput>, IOutputPipeFilter<TInput, TOutput>, IFilter<TInput>, IProbeSite, IPipeConnector<TOutput>, IFilterObserverConnector<TOutput>, IOutputPipeFilter<TInput, TOutput, TKey>, IKeyPipeConnector<TKey>
```

#### Type Parameters

`TInput`<br/>

`TOutput`<br/>

`TKey`<br/>

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [OutputPipeFilter\<TInput, TOutput\>](../masstransit-middleware/outputpipefilter-2) → [OutputPipeFilter\<TInput, TOutput, TKey\>](../masstransit-middleware/outputpipefilter-3)<br/>
Implements [IOutputPipeFilter\<TInput, TOutput\>](../masstransit-middleware/ioutputpipefilter-2), [IFilter\<TInput\>](../../masstransit-abstractions/masstransit/ifilter-1), [IProbeSite](../../masstransit-abstractions/masstransit/iprobesite), [IPipeConnector\<TOutput\>](../masstransit-middleware/ipipeconnector-1), [IFilterObserverConnector\<TOutput\>](../../masstransit-abstractions/masstransit/ifilterobserverconnector-1), [IOutputPipeFilter\<TInput, TOutput, TKey\>](../masstransit-middleware/ioutputpipefilter-3), [IKeyPipeConnector\<TKey\>](../masstransit-middleware/ikeypipeconnector-1)

## Constructors

### **OutputPipeFilter(IPipeContextConverter\<TInput, TOutput\>, FilterObservable, KeyAccessor\<TInput, TKey\>)**

```csharp
public OutputPipeFilter(IPipeContextConverter<TInput, TOutput> contextConverter, FilterObservable observers, KeyAccessor<TInput, TKey> keyAccessor)
```

#### Parameters

`contextConverter` [IPipeContextConverter\<TInput, TOutput\>](../masstransit-middleware/ipipecontextconverter-2)<br/>

`observers` [FilterObservable](../../masstransit-abstractions/masstransit-observables/filterobservable)<br/>

`keyAccessor` [KeyAccessor\<TInput, TKey\>](../masstransit-middleware/keyaccessor-2)<br/>

## Methods

### **ConnectPipe\<T\>(TKey, IPipe\<T\>)**

```csharp
public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`key` TKey<br/>

`pipe` [IPipe\<T\>](../../masstransit-abstractions/masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../../masstransit-abstractions/masstransit/connecthandle)<br/>
