How to do Request/Response with MassTransit
===========================================

This is a super simple example. I just hacked it up so that we would have
something to improve upon later. :)

.. sourcecode:: csharp
    :linenos:
    
    public class BasicRequest { public string Text { get; set; } }
    public class BasicResponse { public string Text { get; set; } }
    public class Program
    {
        public static void Main()
        {
            Bus.Initialize(sbc =>
            {
                sbc.UseMsmq();
                sbc.VerifyMsmqConfiguration();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/test_queue");
                sbc.Subscribe(subs=>
                {
                    subs.Handler<RequestMessage>(msg=> Bus.Instance.Publish(new BasiceResponse{Text = "RESP"+msg.Text}));
                });
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