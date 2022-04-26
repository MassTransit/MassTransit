namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Transports;
    using Util;


    public class ServiceBusQueueMoveTransport
    {
        readonly Recycle<ISendEndpointContextSupervisor> _sendEndpointContext;

        protected ServiceBusQueueMoveTransport(IConnectionContextSupervisor supervisor, SendSettings settings)
        {
            _sendEndpointContext = new Recycle<ISendEndpointContextSupervisor>(() => supervisor.CreateSendEndpointContextSupervisor(settings));
        }

        protected Task Move(ReceiveContext context, Action<ServiceBusMessage, IDictionary<string, object>> preSend)
        {
            IPipe<SendEndpointContext> clientPipe = Pipe.ExecuteAsync<SendEndpointContext>(async clientContext =>
            {
                if (!context.TryGetPayload(out ServiceBusMessageContext messageContext))
                    throw new ArgumentException("The ReceiveContext must contain a BrokeredMessageContext (from Azure Service Bus)", nameof(context));

                var body = context.GetBody();

                var message = new ServiceBusMessage(body)
                {
                    ContentType = context.ContentType?.MediaType,
                    TimeToLive = messageContext.TimeToLive,
                    CorrelationId = messageContext.CorrelationId,
                    MessageId = messageContext.MessageId,
                    Subject = messageContext.Label,
                    PartitionKey = messageContext.PartitionKey,
                    ReplyTo = messageContext.ReplyTo
                };

                if (!string.IsNullOrWhiteSpace(messageContext.SessionId))
                    message.SessionId = messageContext.SessionId;
                if (!string.IsNullOrWhiteSpace(messageContext.ReplyToSessionId))
                    message.ReplyToSessionId = messageContext.ReplyToSessionId;

                foreach (KeyValuePair<string, object> property in messageContext.Properties.Where(x => !x.Key.StartsWith("MT-")))
                    message.ApplicationProperties.Set(new HeaderValue(property.Key, property.Value));

                message.ApplicationProperties.SetHostHeaders();

                preSend(message, message.ApplicationProperties);

                await clientContext.Send(message).ConfigureAwait(false);

                var reason = message.ApplicationProperties.TryGetValue(MessageHeaders.Reason, out var reasonProperty) ? reasonProperty.ToString() : "";
                if (reason == "fault")
                    reason = message.ApplicationProperties.TryGetValue(MessageHeaders.FaultMessage, out var fault) ? $"Fault: {fault}" : "Fault";

                context.LogMoved(clientContext.EntityPath, reason);
            });

            return _sendEndpointContext.Supervisor.Send(clientPipe, context.CancellationToken);
        }
    }
}
