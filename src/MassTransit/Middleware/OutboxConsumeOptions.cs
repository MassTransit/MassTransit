namespace MassTransit.Middleware
{
    using System;


    public class OutboxConsumeOptions
    {
        public Guid ConsumerId { get; set; }
    }
}
