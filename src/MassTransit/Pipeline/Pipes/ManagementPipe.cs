namespace MassTransit.Pipeline.Pipes
{
    using System;
    using Context.Converters;
    using GreenPipes;
    using GreenPipes.Pipes;


    public class ManagementPipe :
        DynamicRouter<ConsumeContext, Guid>,
        IManagementPipe
    {
        public ManagementPipe()
            : base(new ConsumeContextConverterFactory(), GetRequestId)
        {
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return ConnectPipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return ConnectPipe(requestId, pipe);
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}
