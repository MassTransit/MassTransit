namespace MassTransit.JunkDrawer
{
    using System;
    using Middleware;


    public class ManagementPipe :
        DynamicRouter<ConsumeContext, Guid>,
        IManagementPipe
    {
        public ManagementPipe()
            : base(new ConsumeContextConverterFactory(), GetRequestId)
        {
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return ConnectPipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return ConnectPipe(pipe);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return ConnectPipe(requestId, pipe);
        }

        static Guid GetRequestId(ConsumeContext context)
        {
            return context.RequestId ?? Guid.Empty;
        }
    }
}
