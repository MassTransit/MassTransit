---

title: IBusInstanceBuilder

---

# IBusInstanceBuilder

Namespace: MassTransit.DependencyInjection

```csharp
public interface IBusInstanceBuilder
```

## Methods

### **GetBusInstanceType\<TBus, TResult\>(IBusInstanceBuilderCallback\<TBus, TResult\>)**

```csharp
TResult GetBusInstanceType<TBus, TResult>(IBusInstanceBuilderCallback<TBus, TResult> callback)
```

#### Type Parameters

`TBus`<br/>

`TResult`<br/>

#### Parameters

`callback` [IBusInstanceBuilderCallback\<TBus, TResult\>](../masstransit-dependencyinjection/ibusinstancebuildercallback-2)<br/>

#### Returns

TResult<br/>
