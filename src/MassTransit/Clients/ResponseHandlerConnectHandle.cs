namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A connection to a request which handles a result, and completes the Task when it's received
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ResponseHandlerConnectHandle<TResponse> :
        HandlerConnectHandle<TResponse>
        where TResponse : class
    {
        readonly TaskCompletionSource<ConsumeContext<TResponse>> _completed;
        readonly ConnectHandle _handle;
        readonly Task _requestTask;

        public ResponseHandlerConnectHandle(ConnectHandle handle, TaskCompletionSource<ConsumeContext<TResponse>> completed, Task requestTask)
        {
            _handle = handle;
            _completed = completed;
            _requestTask = requestTask;

            Task = GetTask();
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public void Disconnect()
        {
            _handle.Disconnect();
        }

        public void TrySetException(Exception exception)
        {
            _completed.TrySetException(exception);
        }

        public void TrySetCanceled(CancellationToken cancellationToken)
        {
            _completed.TrySetCanceled(cancellationToken);
        }

        public Task<Response<TResponse>> Task { get; }

        async Task<Response<TResponse>> GetTask()
        {
            await _requestTask.ConfigureAwait(false);

            ConsumeContext<TResponse> context = await _completed.Task.ConfigureAwait(false);

            return new MessageResponse<TResponse>(context);
        }
    }
}
