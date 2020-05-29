namespace MassTransit.Transformation.TransformConfigurators
{
    using GreenPipes;
    using Initializers;
    using Pipeline.Filters;


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
