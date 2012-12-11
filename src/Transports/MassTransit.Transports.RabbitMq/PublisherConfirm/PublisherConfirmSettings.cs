namespace MassTransit.Transports.RabbitMq.PublisherConfirm
{
    using System;

    public class PublisherConfirmSettings : IPublisherConfirmSettings
    {
        public const string ClientMessageId = "ClientMessageId";

        public bool UsePublisherConfirms { get; set; }

        public Action<ulong, string> RegisterMessageAction { get; set; }

        public Action<ulong, bool> Acktion { get; set; }

        public Action<ulong, bool> Nacktion { get; set; }

    }
}