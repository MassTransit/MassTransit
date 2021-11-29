namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class RabbitMqReceiveLockContext :
        ReceiveLockContext
    {
        readonly ulong _deliveryTag;
        readonly ModelContext _model;

        public RabbitMqReceiveLockContext(ModelContext model, ulong deliveryTag)
        {
            _model = model;
            _deliveryTag = deliveryTag;
        }

        public async Task Complete()
        {
            try
            {
                await _model.BasicAck(_deliveryTag, false).ConfigureAwait(false);
            }
            catch (InvalidOperationException exception)
            {
                throw new TransportUnavailableException($"Message ACK failed: {_deliveryTag}", exception);
            }
        }

        public async Task Faulted(Exception exception)
        {
            try
            {
                await _model.BasicNack(_deliveryTag, false, true).ConfigureAwait(false);
            }
            catch (Exception ackEx)
            {
                LogContext.Error?.Log(ackEx, "Message NACK failed: {DeliveryTag}, Original Exception: {Exception}", _deliveryTag, exception);
            }
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
