#nullable enable
namespace MassTransit.Tests.ReliableMessaging
{
    using System;


    public class Event
    {
        public Guid MessageId { get; set; }
        public string? Text { get; set; }
    }
}
