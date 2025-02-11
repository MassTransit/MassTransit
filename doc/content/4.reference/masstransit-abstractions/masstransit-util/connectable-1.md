---

title: Connectable<T>

---

# Connectable\<T\>

Namespace: MassTransit.Util

Maintains a collection of connections of the generic type

```csharp
public class Connectable<T>
```

#### Type Parameters

`T`<br/>
The connectable type

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [Connectable\<T\>](../masstransit-util/connectable-1)

## Properties

### **Connected**

```csharp
public T[] Connected { get; }
```

#### Property Value

T[]<br/>

### **Count**

The number of connections

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **Connectable()**

```csharp
public Connectable()
```

## Methods

### **Connect(T)**

Connect a connectable type

```csharp
public ConnectHandle Connect(T connection)
```

#### Parameters

`connection` T<br/>
The connection to add

#### Returns

[ConnectHandle](../masstransit/connecthandle)<br/>
The connection handle

### **ForEachAsync(Func\<T, Task\>)**

Enumerate the connections invoking the callback for each connection

```csharp
public Task ForEachAsync(Func<T, Task> callback)
```

#### Parameters

`callback` [Func\<T, Task\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>
The callback

#### Returns

[Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br/>
An awaitable Task for the operation

### **ForEach(Action\<T\>)**

```csharp
public void ForEach(Action<T> callback)
```

#### Parameters

`callback` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.action-1)<br/>

### **All(Func\<T, Boolean\>)**

```csharp
public bool All(Func<T, bool> callback)
```

#### Parameters

`callback` [Func\<T, Boolean\>](https://learn.microsoft.com/en-us/dotnet/api/system.func-2)<br/>

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Method1()**

```csharp
public void Method1()
```

### **Method2()**

```csharp
public void Method2()
```

### **Method3()**

```csharp
public void Method3()
```
