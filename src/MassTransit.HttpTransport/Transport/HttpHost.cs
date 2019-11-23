namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Hosting;
    using Logging;
    using MassTransit.Topology;
    using Transports;
    using Util;


    public class HttpHost :
        BaseHost,
        IHttpHostControl
    {
        readonly IHttpHostConfiguration _hostConfiguration;
        readonly IHostTopology _hostTopology;
        readonly HttpHostContextSupervisor _httpHostContextSupervisor;

        public HttpHost(IHttpHostConfiguration hostConfiguration, IHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            _httpHostContextSupervisor = new HttpHostContextSupervisor(hostConfiguration);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "HTTP",
                Settings.Host,
                Settings.Port,
                Settings.Method.Method
            });
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            throw new NotImplementedException();
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<HostHandle> Start(CancellationToken cancellationToken)
        {
            LogContext.Debug?.Log("Starting host: {HostAddress}", _hostConfiguration.HostAddress);

            DefaultLogContext = LogContext.Current;
            SendLogContext = LogContext.Current.CreateLogContext(LogCategoryName.Transport.Send);
            ReceiveLogContext = LogContext.Current.CreateLogContext(LogCategoryName.Transport.Receive);

            var handlesReady = TaskUtil.GetTask<HostReceiveEndpointHandle[]>();
            var hostStarted = TaskUtil.GetTask<bool>();

            IPipe<HttpHostContext> connectionPipe = Pipe.ExecuteAsync<HttpHostContext>(async context =>
            {
                try
                {
                    await handlesReady.Task.ConfigureAwait(false);

                    await context.Start(context.CancellationToken).ConfigureAwait(false);

                    hostStarted.TrySetResult(true);

                    await Completed.ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    hostStarted.TrySetException(ex);
                }
                catch (Exception ex)
                {
                    hostStarted.TrySetException(ex);
                    throw;
                }
            });

            var connectionTask = HttpHostContextSupervisor.Send(connectionPipe, Stopping);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints(cancellationToken);

            HostHandle hostHandle = new StartHostHandle(this, handles, GetAgentHandles());

            await hostHandle.Ready.ConfigureAwait(false);

            handlesReady.TrySetResult(handles);

            await hostStarted.Task.ConfigureAwait(false);

            return hostHandle;
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {_httpHostContextSupervisor};
        }

        public IHttpHostContextSupervisor HttpHostContextSupervisor => _httpHostContextSupervisor;
        public HttpHostSettings Settings => _hostConfiguration.Settings;

        public Task<ISendTransport> CreateSendTransport(Uri address, ReceiveEndpointContext receiveEndpointContext)
        {
            var clientContextSupervisor = new HttpClientContextSupervisor(receiveEndpointContext.ReceivePipe);

            var sendSettings = address.GetSendSettings();

            var transport = new HttpSendTransport(clientContextSupervisor, sendSettings, receiveEndpointContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }
    }
}
