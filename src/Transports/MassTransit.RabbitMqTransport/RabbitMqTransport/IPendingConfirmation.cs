namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;


    public interface IPendingConfirmation
    {
        ulong PublishTag { get; }

        /// <summary>
        /// Completed when the publish has been confirmed by the broker
        /// </summary>
        Task Confirmed { get; }

        void Acknowledged();
        void NotAcknowledged();
        void NotConfirmed(Exception exception);
        void NotConfirmed(string reason);
        void Returned(ushort code, string text);
    }
}
