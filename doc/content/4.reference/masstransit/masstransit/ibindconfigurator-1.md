---

title: IBindConfigurator<TLeft>

---

# IBindConfigurator\<TLeft\>

Namespace: MassTransit

```csharp
public interface IBindConfigurator<TLeft>
```

#### Type Parameters

`TLeft`<br/>

## Methods

### **Source\<T\>(IPipeContextSource\<T, TLeft\>, Action\<IBindConfigurator\<TLeft, T\>\>)**

Specifies a pipe context source which is used to create the PipeContext bound to the BindContext.

```csharp
void Source<T>(IPipeContextSource<T, TLeft> source, Action<IBindConfigurator<TLeft, T>> configureTarget)
```

#### Type Parameters

`T`<br/>

#### Parameters

`source` [IPipeContextSource\<T, TLeft\>](../../masstransit-abstractions/masstransit/ipipecontextsource-2)<br/>

`configureTarget` [Action\<IBindConfigurator\<TLeft, T\>\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>
