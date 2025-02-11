---

title: IPropertyInitializerInspector<TMessage, TInput>

---

# IPropertyInitializerInspector\<TMessage, TInput\>

Namespace: MassTransit.Initializers.Factories

```csharp
public interface IPropertyInitializerInspector<TMessage, TInput>
```

#### Type Parameters

`TMessage`<br/>

`TInput`<br/>

## Methods

### **Apply(IMessageInitializerBuilder\<TMessage, TInput\>, IInitializerConvention)**

```csharp
bool Apply(IMessageInitializerBuilder<TMessage, TInput> builder, IInitializerConvention convention)
```

#### Parameters

`builder` [IMessageInitializerBuilder\<TMessage, TInput\>](../masstransit-initializers-factories/imessageinitializerbuilder-2)<br/>

`convention` [IInitializerConvention](../masstransit-initializers-conventions/iinitializerconvention)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
