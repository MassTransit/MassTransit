namespace MassTransit.Context
{
    using System;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class CorrelationIdConsumeContextProxy<TMessage> :
        ConsumeContextProxy<TMessage>
        where TMessage : class
    {
        readonly Guid _correlationId;

        public CorrelationIdConsumeContextProxy(ConsumeContext<TMessage> context, Guid correlationId)
            : base(context)
        {
            _correlationId = correlationId;
        }

        public override Guid? CorrelationId => _correlationId;

        public void Method4()
        {
        }

        public void Method5()
        {
        }

        public void Method6()
        {
        }
    }
}
