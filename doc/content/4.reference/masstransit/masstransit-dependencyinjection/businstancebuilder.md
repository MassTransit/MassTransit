---

title: BusInstanceBuilder

---

# BusInstanceBuilder

Namespace: MassTransit.DependencyInjection

```csharp
public class BusInstanceBuilder : IBusInstanceBuilder
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BusInstanceBuilder](../masstransit-dependencyinjection/businstancebuilder)<br/>
Implements [IBusInstanceBuilder](../masstransit-dependencyinjection/ibusinstancebuilder)

## Fields

### **Instance**

```csharp
public static IBusInstanceBuilder Instance;
```

## Methods

### **GetBusInstanceType\<TBus, TResult\>(IBusInstanceBuilderCallback\<TBus, TResult\>)**

```csharp
public TResult GetBusInstanceType<TBus, TResult>(IBusInstanceBuilderCallback<TBus, TResult> callback)
```

#### Type Parameters

`TBus`<br/>

`TResult`<br/>

#### Parameters

`callback` [IBusInstanceBuilderCallback\<TBus, TResult\>](../masstransit-dependencyinjection/ibusinstancebuildercallback-2)<br/>

#### Returns

TResult<br/>
