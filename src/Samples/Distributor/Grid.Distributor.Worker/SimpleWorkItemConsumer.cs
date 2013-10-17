namespace Grid.Distributor.Worker
{
    using System;
    using MassTransit;
    using MassTransit.Logging;
    using Shared.Messages;


    public class SimpleWorkItemConsumer :
        Consumes<DoSimpleWorkItem>.Context
    {
        static readonly ILog _log = Logger.Get(typeof(SimpleWorkItemConsumer));

        public void Consume(IConsumeContext<DoSimpleWorkItem> context)
        {
            _log.InfoFormat("Responding to {0}", context.Message.CorrelationId);

            context.Respond<CompletedSimpleWorkItem>(new CompletedSimpleWorkItemImpl(context.Message.CorrelationId, context.Message.CreatedAt));
        }


        public class CompletedSimpleWorkItemImpl :
            CompletedSimpleWorkItem
        {
            public CompletedSimpleWorkItemImpl(Guid correlationId, DateTime createdAt)
            {
                CorrelationId = correlationId;
                CreatedAt = DateTime.UtcNow;
                RequestCreatedAt = createdAt;
            }

            public Guid CorrelationId { get; private set; }
            public DateTime CreatedAt { get; private set; }
            public DateTime RequestCreatedAt { get; private set; }
        }
    }
}