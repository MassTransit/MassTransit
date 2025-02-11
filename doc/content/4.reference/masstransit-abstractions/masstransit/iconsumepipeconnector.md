---

title: IConsumePipeConnector

---

# IConsumePipeConnector

Namespace: MassTransit

```csharp
public interface IConsumePipeConnector
```

## Methods

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>)**

```csharp
ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>

### **ConnectConsumePipe\<T\>(IPipe\<ConsumeContext\<T\>\>, ConnectPipeOptions)**

```csharp
ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
```

#### Type Parameters

`T`<br/>

#### Parameters

`pipe` [IPipe\<ConsumeContext\<T\>\>](../masstransit/ipipe-1)<br/>

`options` [ConnectPipeOptions](../masstransit/connectpipeoptions)<br/>

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
