namespace MassTransit.Transports.RabbitMq.PublisherConfirm
{
    using System;

    public interface IPublisherConfirmSettings
    {
        bool UsePublisherConfirms { get; set; }

        Action<ulong, string> RegisterMessageAction { get; set; }

        Action<ulong, bool> Acktion { get; set; }

        Action<ulong, bool> Nacktion { get; set; }

    }
}