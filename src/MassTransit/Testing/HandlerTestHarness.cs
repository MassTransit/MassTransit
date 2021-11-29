namespace MassTransit.Testing
{
    using System;
    using System.Threading.Tasks;


    public class HandlerTestHarness<TMessage>
        where TMessage : class
    {
        readonly ReceivedMessageList<TMessage> _consumed;
        readonly MessageHandler<TMessage> _handler;

        public HandlerTestHarness(BusTestHarness testHarness, MessageHandler<TMessage> handler)
        {
            _handler = handler;

            _consumed = new ReceivedMessageList<TMessage>(testHarness.TestTimeout, testHarness.InactivityToken);

            testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
        }

        public IReceivedMessageList<TMessage> Consumed => _consumed;

        void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<TMessage>(HandleMessage);
        }

        async Task HandleMessage(ConsumeContext<TMessage> context)
        {
            try
            {
                await _handler(context).ConfigureAwait(false);

                _consumed.Add(context);
            }
            catch (Exception ex)
            {
                _consumed.Add(context, ex);
            }
        }
    }
}
