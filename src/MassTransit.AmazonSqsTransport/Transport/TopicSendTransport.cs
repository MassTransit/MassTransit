namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class TopicSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly string _entityName;
        readonly IFilter<ClientContext> _filter;
        readonly IPipeContextSource<ClientContext> _clientSource;
        readonly SendObservable _observers;

        public TopicSendTransport(IPipeContextSource<ClientContext> clientSource, IFilter<ClientContext> preSendFilter, string entityName)
        {
            _clientSource = clientSource;
            _filter = preSendFilter;
            _entityName = entityName;

            _observers = new SendObservable();
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_entityName}");

            IPipe<ClientContext> modelPipe = Pipe.New<ClientContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async clientContext =>
                {
                    var sendContext = new TransportAmazonSqsSendContext<T>(message, cancellationToken);
                    try
                    {
                        await pipe.Send(sendContext).ConfigureAwait(false);

                        var transportMessage = clientContext.CreatePublishRequest(_entityName, sendContext.Body);

                        transportMessage.MessageAttributes.Set(sendContext.Headers);

                        transportMessage.MessageAttributes.Set("Content-Type", sendContext.ContentType.MediaType);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.MessageId), sendContext.MessageId);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.CorrelationId), sendContext.CorrelationId);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.TimeToLive), sendContext.TimeToLive);

                        await _observers.PreSend(sendContext).ConfigureAwait(false);

                        await clientContext.Publish(transportMessage, sendContext.CancellationToken).ConfigureAwait(false);

                        sendContext.LogSent();

                        await _observers.PostSend(sendContext).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        sendContext.LogFaulted(ex);

                        await _observers.SendFault(sendContext, ex).ConfigureAwait(false);

                        throw;
                    }
                });
            });

            await _clientSource.Send(modelPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}