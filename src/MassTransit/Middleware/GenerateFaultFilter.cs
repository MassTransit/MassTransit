namespace MassTransit.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Events;
    using Metadata;


    /// <summary>
    /// Generates and publishes a <see cref="Fault" /> event for the exception
    /// </summary>
    public class GenerateFaultFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("generateFault");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            if (!context.IsFaulted)
            {
                await GenerateFault(context).ConfigureAwait(false);

                await context.NotifyFaulted(context.Exception).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        static async Task GenerateFault(ExceptionReceiveContext context)
        {
            Guid? messageId;
            Guid? requestId;
            string[] messageTypes = null;

            if (context.TryGetPayload(out ConsumeContext consumeContext))
            {
                messageId = consumeContext.MessageId;
                requestId = consumeContext.RequestId;
                messageTypes = consumeContext.SupportedMessageTypes.ToArray();
            }
            else
            {
                messageId = context.GetMessageId();
                requestId = context.GetRequestId();
            }

            if (context.PublishFaults || consumeContext?.FaultAddress != null || consumeContext?.ResponseAddress != null)
            {
                ReceiveFault fault = new ReceiveFaultEvent(HostMetadataCache.Host, context.Exception, context.ContentType?.MediaType, messageId, messageTypes);

                var faultEndpoint = await context.GetReceiveFaultEndpoint(consumeContext, requestId).ConfigureAwait(false);

                await faultEndpoint.Send(fault).ConfigureAwait(false);
            }
        }
    }
}
