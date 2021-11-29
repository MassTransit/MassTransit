namespace MassTransit.Transformation
{
    using System;
    using Initializers;


    public class ReplaceMessageFactory<TMessage> :
        IMessageFactory<TMessage>
        where TMessage : class
    {
        public InitializeContext<TMessage> Create(InitializeContext context)
        {
            if (context.TryGetPayload(out TransformContext<TMessage> transformContext) && transformContext.HasInput)
                return context.CreateMessageContext(transformContext.Input);

            throw new InvalidOperationException("The original message context was not available.");
        }
    }
}
