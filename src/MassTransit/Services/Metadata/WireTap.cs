namespace MassTransit.Services.Metadata
{
    using System;
    using Pipeline;
    using Pipeline.Sinks;

    public class WireTap<TMessage> :
        MessageFilter<TMessage> where TMessage : class
    {
        private readonly Action<TMessage> _action;

        public WireTap(string description, 
            Func<IPipelineSink<TMessage>, IPipelineSink<TMessage>> insertAfter,
            Func<TMessage, bool> allow,
            Action<TMessage> action) :
            base(description, insertAfter, allow)
        {
            _action = action;

            ReplaceOutputSink(insertAfter(this));
        }
    }
}