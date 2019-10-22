namespace MassTransit.Transformation.TransformConfigurators
{
    using GreenPipes;
    using Pipeline.Filters;


    public class ConsumeTransformSpecification<TMessage> :
        TransformSpecification<TMessage>,
        IConsumeTransformSpecification<TMessage>
        where TMessage : class
    {
        void IPipeSpecification<ConsumeContext<TMessage>>.Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            var initializer = Build();

            builder.AddFilter(new TransformFilter<TMessage>(initializer));
        }
    }
}
