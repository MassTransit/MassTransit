namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class MissingConsumeContext :
        ConsumeContext
    {
        bool PipeContext.HasPayloadType(Type payloadType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        bool PipeContext.TryGetPayload<T>(out T payload)
        {
            throw new ConsumeContextNotAvailableException();
        }

        T PipeContext.GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            throw new ConsumeContextNotAvailableException();
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            throw new ConsumeContextNotAvailableException();
        }

        CancellationToken PipeContext.CancellationToken => throw new ConsumeContextNotAvailableException();

        Guid? MessageContext.MessageId => throw new ConsumeContextNotAvailableException();

        Guid? MessageContext.RequestId => throw new ConsumeContextNotAvailableException();

        Guid? MessageContext.CorrelationId => throw new ConsumeContextNotAvailableException();

        Guid? MessageContext.ConversationId => throw new ConsumeContextNotAvailableException();

        Guid? MessageContext.InitiatorId => throw new ConsumeContextNotAvailableException();

        DateTime? MessageContext.ExpirationTime => throw new ConsumeContextNotAvailableException();

        Uri MessageContext.SourceAddress => throw new ConsumeContextNotAvailableException();

        Uri MessageContext.DestinationAddress => throw new ConsumeContextNotAvailableException();

        Uri MessageContext.ResponseAddress => throw new ConsumeContextNotAvailableException();

        Uri MessageContext.FaultAddress => throw new ConsumeContextNotAvailableException();

        DateTime? MessageContext.SentTime => throw new ConsumeContextNotAvailableException();

        Headers MessageContext.Headers => throw new ConsumeContextNotAvailableException();

        HostInfo MessageContext.Host => throw new ConsumeContextNotAvailableException();

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new ConsumeContextNotAvailableException();
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            throw new ConsumeContextNotAvailableException();
        }

        ReceiveContext ConsumeContext.ReceiveContext => throw new ConsumeContextNotAvailableException();

        Task ConsumeContext.ConsumeCompleted => throw new ConsumeContextNotAvailableException();

        IEnumerable<string> ConsumeContext.SupportedMessageTypes => throw new ConsumeContextNotAvailableException();

        bool ConsumeContext.HasMessageType(Type messageType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        bool ConsumeContext.TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            throw new ConsumeContextNotAvailableException();
        }

        void ConsumeContext.AddConsumeTask(Task task)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(T message)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync(object message)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync(object message, Type messageType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(object values)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
        {
            throw new ConsumeContextNotAvailableException();
        }

        void ConsumeContext.Respond<T>(T message)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            throw new ConsumeContextNotAvailableException();
        }

        Task ConsumeContext.NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            throw new ConsumeContextNotAvailableException();
        }
    }
}
