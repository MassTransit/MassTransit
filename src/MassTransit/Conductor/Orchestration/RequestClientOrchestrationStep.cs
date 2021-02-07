namespace MassTransit.Conductor.Orchestration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Registration;


    public class RequestClientOrchestrationStep<TRequest, TResponse, TResult> :
        IOrchestrationStep<TRequest, TResponse, TResult>
        where TRequest : class
        where TResponse : class
        where TResult : class
    {
        readonly Uri _inputAddress;
        readonly RequestTimeout _timeout;

        public RequestClientOrchestrationStep(Uri inputAddress)
        {
            _inputAddress = inputAddress;
            _timeout = RequestTimeout.Default;
        }

        public async Task<TResult> Execute(OrchestrationContext<TRequest> context, IOrchestration<TResponse, TResult> next)
        {
            LogContext.Debug?.Log("RequestClient<{RequestType}, {ResponseType}", TypeCache<TRequest>.ShortName, TypeCache<TResponse>.ShortName);

            var provider = context.GetPayload<IScopeServiceProvider>();

            var clientFactory = provider.GetRequiredService<IClientFactory>();

            IRequestClient<TRequest> client = _inputAddress != null
                ? clientFactory.CreateRequestClient<TRequest>(context, _inputAddress, _timeout)
                : clientFactory.CreateRequestClient<TRequest>(context, _timeout);

            Response<TResponse> response = await client.GetResponse<TResponse>(context.Data).ConfigureAwait(false);

            OrchestrationContext<TResponse> nextContext = context.Push(response.Message);

            return await next.Execute(nextContext).ConfigureAwait(false);
        }
    }
}
