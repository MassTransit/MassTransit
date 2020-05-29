namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class SendValidationEmail :
        CorrelatedMessage
    {
        public SendValidationEmail(Guid correlationId)
            :
            base(correlationId)
        {
        }

        protected SendValidationEmail()
        {
        }
    }
}
