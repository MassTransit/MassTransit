namespace MassTransit.Conductor.Messages
{
    using System;
    using Contracts;


    public class LinkMessage<T> :
        Link<T>
        where T : class
    {
        public LinkMessage(Guid clientId)
        {
            ClientId = clientId;
        }

        public Guid ClientId { get; }
    }
}
