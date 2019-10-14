namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Util;


    public class HttpClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        readonly HttpClient _client;
        readonly IReceivePipe _receivePipe;

        public HttpClientContext(HttpClient client, IReceivePipe receivePipe, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _client = client;
            _receivePipe = receivePipe;
        }

        public Uri BaseAddress => _client.BaseAddress;

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _client.SendAsync(request, cancellationToken);
        }

        public Task ReceiveResponse(ReceiveContext receiveContext)
        {
            return _receivePipe.Send(receiveContext);
        }

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            try
            {
                _client.CancelPendingRequests();

                _client.Dispose();

                LogContext.Debug?.Log("Closed client: {Host}", _client.BaseAddress);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to close client: {Host}", _client.BaseAddress);
            }

            return TaskUtil.Completed;
        }
    }
}
