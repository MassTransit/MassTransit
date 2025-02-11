---

title: INewIdGenerator

---

# INewIdGenerator

Namespace: MassTransit

```csharp
public interface INewIdGenerator
```

## Methods

### **Next()**

```csharp
NewId Next()
```

#### Returns

[NewId](../masstransit/newid)<br/>

### **Next(NewId[], Int32, Int32)**

```csharp
ArraySegment<NewId> Next(NewId[] ids, int index, int count)
```

#### Parameters

`ids` [NewId[]](../masstransit/newid)<br/>

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[ArraySegment\<NewId\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

### **NextGuid()**

```csharp
Guid NextGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

### **NextSequentialGuid(Guid[], Int32, Int32)**

```csharp
ArraySegment<Guid> NextSequentialGuid(Guid[] ids, int index, int count)
```

#### Parameters

`ids` [Guid[]](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>

`index` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

`count` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

#### Returns

[ArraySegment\<Guid\>](https://learn.microsoft.com/en-us/dotnet/api/system.arraysegment-1)<br/>

### **NextSequentialGuid()**

```csharp
Guid NextSequentialGuid()
```

#### Returns

[Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid)<br/>
