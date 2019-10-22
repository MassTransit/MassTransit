namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Pipeline;
    using Transports;


    public class BrokeredMessageMoveTransport
    {
        readonly ISendEndpointContextSupervisor _supervisor;

        protected BrokeredMessageMoveTransport(ISendEndpointContextSupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        protected Task Move(ReceiveContext context, Action<Message, SendHeaders> preSend)
        {
            IPipe<SendEndpointContext> clientPipe = Pipe.ExecuteAsync<SendEndpointContext>(async clientContext =>
            {
                if (!context.TryGetPayload(out BrokeredMessageContext messageContext))
                    throw new ArgumentException("The ReceiveContext must contain a BrokeredMessageContext (from Azure Service Bus)", nameof(context));

                using (var messageBodyStream = context.GetBodyStream())
                {
                    var message = new Message(messageBodyStream.ReadAsBytes())
                    {
                        ContentType = context.ContentType.MediaType,
                        TimeToLive = messageContext.TimeToLive,
                        CorrelationId = messageContext.CorrelationId,
                        MessageId = messageContext.MessageId,
                        Label = messageContext.Label,
                        PartitionKey = messageContext.PartitionKey,
                        ReplyTo = messageContext.ReplyTo,
                        ReplyToSessionId = messageContext.ReplyToSessionId,
                        SessionId = messageContext.SessionId
                    };

                    SendHeaders headers = new DictionarySendHeaders(message.UserProperties);

                    foreach (KeyValuePair<string, object> property in messageContext.Properties)
                        headers.Set(property.Key, property.Value);

                    headers.SetHostHeaders();

                    preSend(message, headers);

                    await clientContext.Send(message).ConfigureAwait(false);

                    var reason = message.UserProperties.TryGetValue(MessageHeaders.Reason, out var reasonProperty) ? reasonProperty.ToString() : "";
                    if (reason == "fault")
                        reason = message.UserProperties.TryGetValue(MessageHeaders.FaultMessage, out var fault) ? $"Fault: {fault}" : "Fault";

                    context.LogMoved(clientContext.EntityPath, reason);
                }
            });

            return _supervisor.Send(clientPipe, context.CancellationToken);
        }
    }
}
