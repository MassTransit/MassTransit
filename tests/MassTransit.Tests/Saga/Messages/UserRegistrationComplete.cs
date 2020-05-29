namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class UserRegistrationComplete :
        CorrelatedMessage
    {
        public UserRegistrationComplete(Guid correlationId)
            :
            base(correlationId)
        {
        }

        protected UserRegistrationComplete()
        {
        }
    }
}
