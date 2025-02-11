---

title: NewIdGenerator

---

# NewIdGenerator

Namespace: MassTransit

```csharp
public class NewIdGenerator : INewIdGenerator
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [NewIdGenerator](../masstransit/newidgenerator)<br/>
Implements [INewIdGenerator](../masstransit/inewidgenerator)

## Constructors

### **NewIdGenerator(ITickProvider, IWorkerIdProvider, IProcessIdProvider, Int32)**

```csharp
public NewIdGenerator(ITickProvider tickProvider, IWorkerIdProvider workerIdProvider, IProcessIdProvider processIdProvider, int workerIndex)
```

#### Parameters

`tickProvider` [ITickProvider](../masstransit/itickprovider)<br/>

`workerIdProvider` [IWorkerIdProvider](../masstransit/iworkeridprovider)<br/>

`processIdProvider` [IProcessIdProvider](../masstransit/iprocessidprovider)<br/>

`workerIndex` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Methods

### **Next()**

```csharp
public NewId Next()
```

#### Returns

[NewId](../masstransit/newid)<br/>

### **NextGuid()**

```csharp
public Guid NextGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **NextSequentialGuid()**

```csharp
public Guid NextSequentialGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **Next(NewId[], Int32, Int32)**

```csharp
public ArraySegment<NewId> Next(NewId[] ids, int index, int count)
```

#### Parameters

`ids` [NewId[]](../masstransit/newid)<br/>

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[ArraySegment\<NewId\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

### **NextSequentialGuid(Guid[], Int32, Int32)**

```csharp
public ArraySegment<Guid> NextSequentialGuid(Guid[] ids, int index, int count)
```

#### Parameters

`ids` [Guid[]](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[ArraySegment\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>
