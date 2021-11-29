namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Connects a message handler to a pipe
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageObserverConnector<TMessage> :
        IObserverConnector<TMessage>
        where TMessage : class
    {
        public ConnectHandle ConnectObserver(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters)
        {
            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                foreach (IFilter<ConsumeContext<TMessage>> filter in filters)
                    x.UseFilter(filter);

                x.AddPipeSpecification(new ObserverPipeSpecification<TMessage>(observer));
            });

            return consumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestObserver(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters)
        {
            IPipe<ConsumeContext<TMessage>> pipe = Pipe.New<ConsumeContext<TMessage>>(x =>
            {
                foreach (IFilter<ConsumeContext<TMessage>> filter in filters)
                    x.UseFilter(filter);

                x.AddPipeSpecification(new ObserverPipeSpecification<TMessage>(observer));
            });

            return consumePipe.ConnectRequestPipe(requestId, pipe);
        }
    }
}
