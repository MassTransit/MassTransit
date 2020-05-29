namespace MassTransit.Transformation.TransformConfigurators
{
    using GreenPipes;
    using Initializers;
    using Pipeline.Filters;


    public class SendTransformSpecification<TMessage> :
        TransformSpecification<TMessage>,
        ISendTransformSpecification<TMessage>
        where TMessage : class
    {
        void IPipeSpecification<SendContext<TMessage>>.Apply(IPipeBuilder<SendContext<TMessage>> builder)
        {
            IMessageInitializer<TMessage> initializer = Build();

            builder.AddFilter(new TransformFilter<TMessage>(initializer));
        }
    }
}
