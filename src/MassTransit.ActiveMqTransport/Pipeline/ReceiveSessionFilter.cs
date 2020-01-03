namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;


    /// <summary>
    /// Creates a receiving model context using the connection
    /// </summary>
    public class ReceiveSessionFilter :
        IFilter<ConnectionContext>
    {
        readonly IActiveMqHost _host;
        readonly IPipe<SessionContext> _pipe;

        public ReceiveSessionFilter(IPipe<SessionContext> pipe, IActiveMqHost host)
        {
            _pipe = pipe;
            _host = host;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("receiveSession");

            _pipe.Probe(scope);
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var session = await context.CreateSession(context.CancellationToken).ConfigureAwait(false);

            var sessionContext = new ActiveMqSessionContext(context, session, context.CancellationToken);

            void HandleException(Exception exception)
            {
            #pragma warning disable 4014
                sessionContext.DisposeAsync(CancellationToken.None);
            #pragma warning restore 4014
            }

            context.Connection.ExceptionListener += HandleException;

            try
            {
                await _pipe.Send(sessionContext).ConfigureAwait(false);
            }
            finally
            {
                context.Connection.ExceptionListener -= HandleException;

                await sessionContext.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
