namespace MassTransit.Configuration
{
    using Initializers;
    using Middleware;


    public class ConsumeTransformSpecification<TMessage> :
        TransformSpecification<TMessage>,
        IConsumeTransformSpecification<TMessage>
        where TMessage : class
    {
        void IPipeSpecification<ConsumeContext<TMessage>>.Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            IMessageInitializer<TMessage> initializer = Build();

            builder.AddFilter(new TransformFilter<TMessage>(initializer));
        }
    }
}
