namespace MassTransit.ServiceBus.Tests.Messages
{
    using System;

    [Serializable]
    public class PoisonMessage : IMessage
    {
        public string ThrowException()
        {
            throw new Exception("POISON"); 
        }
    }
}