namespace MassTransit.Tests.Saga.Messages
{
    using System;


    [Serializable]
    public class SendUserVerificationEmail :
        CorrelatedMessage
    {
        public SendUserVerificationEmail(Guid correlationId, string email)
            :
            base(correlationId)
        {
            Email = email;
        }

        protected SendUserVerificationEmail()
        {
        }

        public string Email { get; set; }
    }
}
