---
layout: default
title: Contexts
subtitle: A unit of interaction with the pipe
---

The `context` is the unit of an interaction with a pipe. Each time you call `pipe.Send` you will be passing an instance of a `PipeContext`. This is not your typical message payload. This is the context for passing through the pipes. If you built a message bus on top of GreenPipes the message would exist in the context, but would not be the context. The context can store data inside of it, and should be considered write only and once a value is written it should be immutable.

> We highly recommend using interfaces with Read Only properties when ever possible.

## Example Context

An example of a `PipeContext` might be easier to digest than the abstract paragraph above. First, we need to create an interface that represents our context. Lets pretend that we are building an email pipeline. Step one is to create the interface we will run our pipeline off of.

```csharp
public interface IEmailSendContext : PipeContext
{
  MailAddress To { get; }
  string Subject { get; }
  string Body { get; }
}
```

Next, we implement the context.

```csharp
public class InMemoryEmailSendContext : BasePipeContext, IEmailSendContext
{
  public MailAddress To { get; set; }
  public string Subject { get; set; }
  public string Body { get; set; }
}
```

We are now ready to send this context down the pipe.

## Overly simple usage

```csharp
var pipe = CustomEmailPipeline.Build();
var context = new InMemoryEmailSendContext("letsdothis@now.com", "Welcome to Now.com", "Let's Go Now!")
await pipe.Send(context);
```

## Loading up a context with HTTP stuffs

Side note, you can load any of your contexts up with HTTP data like so.

```csharp
var httpContext = new HttpContext(this.Request, this.Response);
pipe.Send(httpContext);
```
