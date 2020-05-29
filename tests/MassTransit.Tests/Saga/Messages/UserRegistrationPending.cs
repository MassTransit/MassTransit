namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class UserRegistrationPending :
        CorrelatedMessage
    {
        public UserRegistrationPending(Guid correlationId)
            :
            base(correlationId)
        {
        }

        protected UserRegistrationPending()
        {
        }
    }
}
