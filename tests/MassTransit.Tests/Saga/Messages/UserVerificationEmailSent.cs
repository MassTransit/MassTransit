namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class UserVerificationEmailSent :
        CorrelatedMessage
    {
        public UserVerificationEmailSent(Guid correlationId, string email)
            :
            base(correlationId)
        {
            Email = email;
        }

        protected UserVerificationEmailSent()
        {
        }

        public string Email { get; set; }
    }
}
