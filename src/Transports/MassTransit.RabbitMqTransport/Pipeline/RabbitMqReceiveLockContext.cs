namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Transports;
    using Util;


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

        public Task Complete()
        {
            return _model.BasicAck(_deliveryTag, false);
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
            return TaskUtil.Completed;
        }
    }
}
