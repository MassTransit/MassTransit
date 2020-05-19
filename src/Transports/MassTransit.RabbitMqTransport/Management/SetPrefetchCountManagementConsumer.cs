namespace MassTransit.RabbitMqTransport.Management
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using MassTransit.Pipeline.Pipes;


    public class SetPrefetchCountManagementConsumer :
        IConsumer<SetPrefetchCount>
    {
        readonly IManagementPipe _managementPipe;
        readonly string _queueName;
        DateTime _lastUpdated;

        public SetPrefetchCountManagementConsumer(IManagementPipe managementPipe, string queueName)
        {
            _managementPipe = managementPipe;
            _queueName = queueName;
            _lastUpdated = DateTime.UtcNow;
        }

        public async Task Consume(ConsumeContext<SetPrefetchCount> context)
        {
            if (_queueName.Equals(context.Message.QueueName, StringComparison.OrdinalIgnoreCase))
            {
                if (context.Message.Timestamp >= _lastUpdated)
                {
                    try
                    {
                        await _managementPipe.Send(context).ConfigureAwait(false);

                        _lastUpdated = context.Message.Timestamp;

                        await context.RespondAsync<PrefetchCountUpdated>(new
                        {
                            Timestamp = DateTime.UtcNow,
                            QueueName = _queueName,
                            context.Message.PrefetchCount
                        }).ConfigureAwait(false);

                        LogContext.Debug?.Log("Set Prefetch Count: (queue: {QueueName}, count: {PrefetchCount})", _queueName, context.Message.PrefetchCount);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Error?.Log(exception, "Set Prefetch Count failed: (queue: {QueueName}, count: {PrefetchCount})", _queueName,
                            context.Message.PrefetchCount);

                        throw;
                    }
                }
                else
                    throw new CommandException("The prefetch count was updated after the command was sent.");
            }
        }
    }
}
