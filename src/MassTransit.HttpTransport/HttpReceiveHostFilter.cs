namespace MassTransit.HttpTransport
{
    using System.Threading.Tasks;
    using Configuration.Builders;
    using Hosting;
    using MassTransit.Pipeline;
    using Util;


    public class HttpReceiveHostFilter : IFilter<OwinHostContext>
    {
        readonly IPipe<OwinHostContext> _pipe;
        readonly ReceiveSettings _settings;
        readonly ITaskSupervisor _supervisor;

        public HttpReceiveHostFilter(IPipe<OwinHostContext> pipe, ITaskSupervisor supervisor, ReceiveSettings settings)
        {
            _pipe = pipe;
            _supervisor = supervisor;
            _settings = settings;
        }

        public void Probe(ProbeContext context)
        {
            //no-op
        }

        public async Task Send(OwinHostContext context, IPipe<OwinHostContext> next)
        {
            using (var scope = _supervisor.CreateScope($"{TypeMetadataCache<HttpReceiveHostFilter>.ShortName}"))
            {
                //TODO: I can't really get a model
//                IModel model = context.Instance.Start(null);
//
//                using (var modelContext = new RabbitMqModelContext(context, model, scope, _settings))
//                {
//                    await _pipe.Send(modelContext).ConfigureAwait(false);
//                }
//
                await scope.Completed.ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
        }
    }
}