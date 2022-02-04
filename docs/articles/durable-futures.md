---
sidebarDepth: 0
---

# Durable Futures

[[toc]]

## Introduction

Durable Futures are a concept I've come up with to address the complexity inherent to a distributed, event-based architecture.

The concepts in this article are covered [in Season 3](https://youtube.com/playlist?list=PLx8uyNNs1ri2JeyDGFWfCYyAjOB1GP-t1) of the [MassTransit video series on YouTube](https://youtube.com/playlist?list=PLx8uyNNs1ri2MBx6BjPum5j9_MMdIfM9C). 

The code exploring the concepts is [available on GitHub](https://github.com/MassTransit/Sample-ForkJoint).

## Request, Response

One of the most understood concepts in software development is request/response. In the simplest form, call/return, this conversation pattern between a client and a service, is the most commonly used idiom in software development.

```cs
var response = service.Method(request);
```

As programming languages have evolved, along with the common use of asynchronous programming models, remote procedure calls (RPC) via HTTP and other protocols, and message-based systems, the most understood pattern continues to be request/response.

#### HTTP Client

```cs
var responseMessage = await httpClient.GetAsync();
```

#### MassTransit Request Client

```cs
var response = await client.GetResponse<TResponse>(new Request());
```

In each of these examples, _await_ is a key enabler. Requests are sent asynchronously over a network connection to the remote service that produces a response which is then delivered to the client. 

With HTTP, a connection is maintained by the client on which the response is sent. With MassTransit, a _requestId_ and _responseAddress_ passed to the service are used to send the response which is then read from the queue by the client bus and correlated back to the request.

![Request Response](/requestResponse.svg "Request Response")

## Task

The return type, `Task<T>`, is a C# language feature that represents a _future_. It's a reference type which means it is only accessible by reference. Since `Task<T>` is a future, it promises to deliver at some point:

- `T` _(completed)_
- An exception _(faulted)_
- A task canceled exception _(canceled)_

_I'm intentionally ignoring `ValueTask<T>` for now. It behaves similarly but has a few restrictions, one of which being that it should only be evaluated once._

::: tip C#
Prior to the addition of `async` and `await`, writing asynchronous code was significantly more complex. _Continuation passing_ was commonly used, resulting in deeply nested code that was difficult to understand and even more difficult to debug. Without a doubt, `async` and `await` are two of the best keywords in C#.
:::
