namespace MassTransit.Tests
{
    public class NewConfigurationModel
    {
        public void bob()
        {
            Bus.Initialize(ep=>
            {
                ////
                //HIGH USAGE OPTIONS 
                ////
                //ep.SetDefaultSerializer();


                //the auto register should be enabled by default.
                //there should be a turn off that requires you to pick the desired transports.
                //ep.RegisterTransport();


                ////
                //ADVANCED OPTION 
                ////
                //ep.ConfigureEndpoint();
                
            },
            bus=>
            {
                ////
                //HARD REQUIREMENTS
                ////
                bus.ReceiveFrom("msmq://localhost/bob");

                ////
                //PROBABLY MOST OFTEN USED CONFIG OPTIONS 
                ////
                //bus.SetObjectBuilder();
                //bus.SendErrorsTo();

                ////
                //These are REAL options - absolutely not required for this to work.
                ////
                bus.DisableAutoStart();
                bus.EnableAutoSubscribe();
                bus.PurgeBeforeStarting();

                ////
                //Extension point 
                ////
                //add bus services? is service the right concept?
                //bus.AddService();

                ////
                //ADVANCED OPTIONS
                ////
                //bus.AfterConsumingMessage();
                //bus.BeforeConsumingMessage();
                //bus.SetConcurrentConsumerLimit();
                //bus.SetConcurrentReceiverLimit();
                //bus.SetReceiveTimeout();
                //bus.UseControlBus();
                
                ////
                //SHOULD BE OBSOLETED 
                ////
                //bus.SetEndpointFactory();
            });
        }
    }
}