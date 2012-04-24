namespace Grid.Distributor.Worker
{
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

            context.Respond(new CompletedSimpleWorkItem(context.Message.CorrelationId, context.Message.CreatedAt));
        }
    }
}