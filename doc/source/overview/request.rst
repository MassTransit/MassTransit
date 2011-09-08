How to do Request/Response with MassTransit
===========================================

This is a super simple example. I just hacked it up so that we would have
something to improve upon later. :)

.. sourcecode:: csharp
    :linenos:

    //the messages
    public class BasicRequest :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get;set; }
        public string Text { get; set; }
    }
    public class BasicResponse :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public string Text { get; set; }
    }

.. sourcecode:: csharp
    :linenos:

    //the responder
    public class Program
    {
        public static void Main()
        {
            Bus.Initialize(sbc =>
            {
                sbc.UseMsmq();
                sbc.VerifyMsmqConfiguration();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/message_responder");
                sbc.Subscribe(subs=>
                {
                    subs.Handler<RequestMessage>(msg=> Bus.Instance.MessageContext<RequestMessage>().Respond(new BasiceResponse{Text = "RESP"+msg.Text}));
                });
            });
        }
    }

.. sourcecode:: csharp
    :linenos:

    //the requester
    public class Program
    {
        public static void Main()
        {
            Bus.Initialize(sbc =>
            {
                sbc.UseMsmq();
                sbc.VerifyMsmqConfiguration();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/message_requestor");
            });

            Bus.Instance.PublishRequest(new RequestMessage(), x =>
            {
                x.Handle<ResponseMessage>(message => Console.WriteLine(message.Text));
                x.SetTimeout(30.Seconds());
            });
        }
    }

So what is going on? The first chunk has the messages we are gonig to work with.

The second chunk shows the code to simple echo back the request message as a response.

The final chunk shows the code to publish the request and handle any responses that relate
to the original request message. Once any response is received (with the same correlation id as
the original request) the remaining handlers are unsubscribed and the request operation completes.

This style of request will block the calling thread until either a response is received by one of
the handlers, or the timeout period expires. If it expires, a RequestTimeoutException is thrown.
If a response handler throws an exception, that exception is rethrown on the thread that sent the
request (since it is blocked waiting on the response anyway).

The request can also be executed asynchronously using the Asychronous Programming Model of .NET.
By calling BeginPublishRequest (or the endpoint-based BeginSendRequest), an IAsyncResult is returned
to the caller. The IAsyncResult could then be passed to whatever framework code is handling the asynchronous
operation (such as a BeginWebMethod/EndWebMethod pair or an AsyncController).

Once the callback is invoked (or the wait handle is signaled), the EndRequest method (which is an extension
method off IEndpoint or IServiceBus) must be called to complete the request (at this point, any timeout or
response handler exceptions will be thrown).

NOTE: The asynchronous model will create a wait event if requested, but the callback style is greatly
preferred since it reduces the amount of operating system resources required.