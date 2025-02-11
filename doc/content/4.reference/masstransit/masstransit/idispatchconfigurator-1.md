---

title: IDispatchConfigurator<TContext>

---

# IDispatchConfigurator\<TContext\>

Namespace: MassTransit

```csharp
public interface IDispatchConfigurator<TContext>
```

#### Type Parameters

`TContext`<br/>

## Methods

### **Pipe\<T\>(Action\<IPipeConfigurator\<T\>\>)**

```csharp
void Pipe<T>(Action<IPipeConfigurator<T>> configurePipe)
```

#### Type Parameters

`T`<br/>

#### Parameters

`configurePipe` [Action\<IPipeConfigurator\<T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
