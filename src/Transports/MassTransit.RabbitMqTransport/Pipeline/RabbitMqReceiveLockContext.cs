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
        readonly ModelContext _model;
        readonly ulong _deliveryTag;

        public RabbitMqReceiveLockContext(ModelContext model, ulong deliveryTag)
        {
            _model = model;
            _deliveryTag = deliveryTag;
        }

        public Task Complete()
        {
            _model.BasicAck(_deliveryTag, false);

            return TaskUtil.Completed;
        }

        public Task Faulted(Exception exception)
        {
            try
            {
                _model.BasicNack(_deliveryTag, false, true);
            }
            catch (Exception ackEx)
            {
                LogContext.Error?.Log(ackEx, "Message NACK failed: {DeliveryTag}, Original Exception: {Exception}", _deliveryTag, exception);
            }

            return TaskUtil.Completed;
        }

        public Task ValidateLockStatus()
        {
            return TaskUtil.Completed;
        }
    }
}
