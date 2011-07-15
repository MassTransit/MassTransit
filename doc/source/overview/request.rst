How to do Request/Response with MassTransit
===========================================

This is a super simple example. I just hacked it up so that we would have
something to improve upon later. :)

.. sourcecode:: csharp
    :linenos:
    
    //the messages
    public class BasicRequest { public string Text { get; set; } }
    public class BasicResponse { public string Text { get; set; } }

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
                    subs.Handler<RequestMessage>(msg=> Bus.Instance.Publish(new BasiceResponse{Text = "RESP"+msg.Text}));
                });
            });
        }
    }

.. sourcecode:: csharp
    :linenos:
    
    //the requestor
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
            
            Bus.Instance.MakeRequest(bus=>
            {
                bus.Publish(new RequestMessage());
            })
            .When<ResponseMessage>()
            .IsReceived(msg => 
            {
                Console.WriteLine(msg.Text);
            })
            .Send();
        }
    }

So what is going on? The first chunk has the messages we are gonig to work with.

The second chunk shows the code to simple echo back the request message as a response.

The last chunk shows the code to 'Make the Request' this includes
setting up a handler for the response.
We tell the bus what the expected return type is ``When<ResponseMessage>`` and then what to
do when it ``IsReceived``. Finally we call send to make it all happen.