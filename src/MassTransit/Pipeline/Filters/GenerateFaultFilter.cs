namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Generates and publishes a <see cref="Fault"/> event for the exception
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
                GenerateFault(context);

                await context.NotifyFaulted(context.Exception).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        static void GenerateFault(ExceptionReceiveContext context)
        {
            IPublishEndpoint publishEndpoint;
            Guid? faultedMessageId;
            string[] faultMessageTypes = null;

            if (context.TryGetPayload(out ConsumeContext consumeContext))
            {
                publishEndpoint = consumeContext;
                faultedMessageId = consumeContext.MessageId;
                faultMessageTypes = consumeContext.SupportedMessageTypes.ToArray();
            }
            else
            {
                faultedMessageId = context.TransportHeaders.Get("MessageId", default(Guid?));

                publishEndpoint = context.PublishEndpointProvider.CreatePublishEndpoint(context.InputAddress);
            }

            ReceiveFault fault = new ReceiveFaultEvent(HostMetadataCache.Host, context.Exception, context.ContentType.MediaType, faultedMessageId,
                faultMessageTypes);

            var contextPipe = new ConsumeSendContextPipe<ReceiveFault>(consumeContext);

            var publishTask = publishEndpoint.Publish(fault, contextPipe, context.CancellationToken);

            context.AddReceiveTask(publishTask);
        }
    }
}
