namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using Transports;


    public class RabbitMqReceiveLockContext :
        ReceiveLockContext
    {
        readonly ChannelContext _channel;
        readonly ulong _deliveryTag;

        public RabbitMqReceiveLockContext(ChannelContext channel, ulong deliveryTag)
        {
            _channel = channel;
            _deliveryTag = deliveryTag;
        }

        public async Task Complete()
        {
            if (_channel.Channel.IsClosed)
            {
                throw new OperationInterruptedException(
                    new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {_channel.Channel.CloseReason}"));
            }

            try
            {
                await _channel.BasicAck(_deliveryTag, false).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new TransportUnavailableException($"Message ACK failed: {_deliveryTag}", exception);
            }
        }

        public async Task Faulted(Exception exception)
        {
            if (_channel.Channel.IsClosed)
            {
                throw new OperationInterruptedException(
                    new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {_channel.Channel.CloseReason}"));
            }

            try
            {
                await _channel.BasicNack(_deliveryTag, false, true).ConfigureAwait(false);
            }
            catch (Exception ackEx)
            {
                LogContext.Error?.Log(ackEx, "Message NACK failed: {DeliveryTag}, Original Exception: {Exception}", _deliveryTag, exception);
            }
        }

        public Task ValidateLockStatus()
        {
            if (_channel.Channel.IsClosed)
            {
                throw new OperationInterruptedException(
                    new ShutdownEventArgs(ShutdownInitiator.Peer, 491, $"Channel is already closed: {_channel.Channel.CloseReason}"));
            }

            return Task.CompletedTask;
        }
    }
}
