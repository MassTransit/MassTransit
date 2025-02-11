---

title: ScopedFilterSpecificationObserver

---

# ScopedFilterSpecificationObserver

Namespace: MassTransit.Configuration

```csharp
public class ScopedFilterSpecificationObserver : ISendPipeSpecificationObserver, IPublishPipeSpecificationObserver
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ScopedFilterSpecificationObserver](../masstransit-configuration/scopedfilterspecificationobserver)<br/>
Implements [ISendPipeSpecificationObserver](../../masstransit-abstractions/masstransit-configuration/isendpipespecificationobserver), [IPublishPipeSpecificationObserver](../../masstransit-abstractions/masstransit-configuration/ipublishpipespecificationobserver)

## Constructors

### **ScopedFilterSpecificationObserver(Type, IServiceProvider, CompositeFilter\<Type\>)**

```csharp
public ScopedFilterSpecificationObserver(Type filterType, IServiceProvider provider, CompositeFilter<Type> messageTypeFilter)
```

#### Parameters

`filterType` [Type](https://learn.microsoft.com/en-us/dotnet/api/system.type)<br/>

`provider` IServiceProvider<br/>

`messageTypeFilter` [CompositeFilter\<Type\>](../masstransit-configuration/compositefilter-1)<br/>

## Methods

### **MessageSpecificationCreated\<T\>(IMessagePublishPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessagePublishPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagepublishpipespecification-1)<br/>

### **MessageSpecificationCreated\<T\>(IMessageSendPipeSpecification\<T\>)**

```csharp
public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
```

#### Type Parameters

`T`<br/>

#### Parameters

`specification` [IMessageSendPipeSpecification\<T\>](../../masstransit-abstractions/masstransit-configuration/imessagesendpipespecification-1)<br/>
