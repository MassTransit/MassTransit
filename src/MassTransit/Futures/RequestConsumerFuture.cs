namespace MassTransit
{
    using System;


    public class RequestConsumerFuture<TRequest, TResponse> :
        Future<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public RequestConsumerFuture(IFutureDefinition definition)
        {
            if (!(definition is IFutureRequestDefinition<TRequest> settings))
            {
                throw new ArgumentException(
                    $"{TypeCache.GetShortName(definition.GetType())} does not implement {TypeCache<IFutureRequestDefinition<TRequest>>.ShortName}");
            }

            SendRequest<TRequest>(x => x.RequestAddress = settings.RequestAddress)
                .OnResponseReceived<TResponse>(x => x.SetCompleted());
        }
    }
}
