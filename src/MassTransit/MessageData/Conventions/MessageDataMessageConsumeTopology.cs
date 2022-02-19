namespace MassTransit.MessageData.Conventions
{
    using Initializers;
    using MassTransit.Configuration;
    using Middleware;


    public class MessageDataMessageConsumeTopology<T> :
        IMessageConsumeTopology<T>
        where T : class
    {
        readonly TransformFilter<T> _transformFilter;

        public MessageDataMessageConsumeTopology(IMessageInitializer<T> initializer)
        {
            _transformFilter = new TransformFilter<T>(initializer);
        }

        public void Apply(ITopologyPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(_transformFilter);
        }
    }
}
