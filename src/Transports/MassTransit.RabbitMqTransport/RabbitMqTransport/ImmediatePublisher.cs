namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using Util;


    public class ImmediatePublisher :
        IPublisher
    {
        readonly PendingConfirmationCollection _confirmations;
        readonly ChannelExecutor _executor;
        readonly IModel _model;

        public ImmediatePublisher(ChannelExecutor executor, IModel model, PendingConfirmationCollection confirmations)
        {
            _executor = executor;
            _model = model;
            _confirmations = confirmations;
        }

        public Task Publish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck)
        {
            return _executor.Run(() =>
            {
                var publishTag = _model.NextPublishSeqNo;

                var published = _confirmations?.Add(exchange, publishTag, basicProperties);
                try
                {
                    _model.BasicPublish(exchange, routingKey, mandatory, basicProperties, body);
                }
                catch (Exception)
                {
                    _confirmations?.Faulted(published);
                    throw;
                }

                return awaitAck && published != null ? published.Confirmed : Task.CompletedTask;
            });
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
