namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class UserValidated :
        CorrelatedMessage
    {
        public UserValidated(Guid correlationId)
            :
            base(correlationId)
        {
        }

        protected UserValidated()
        {
        }
    }
}
