namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using Pipeline;
    using Util;


    /// <summary>
    /// Connects a handler to the inbound pipe of the receive endpoint
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class ResponseHandlerConfigurator<TResponse> :
        IHandlerConfigurator<TResponse>
        where TResponse : class
    {
        readonly MessageHandler<TResponse> _handler;
        readonly Task _requestTask;
        readonly IList<IPipeSpecification<ConsumeContext<TResponse>>> _specifications;
        readonly TaskCompletionSource<ConsumeContext<TResponse>> _completed;
        readonly TaskScheduler _taskScheduler;

        public ResponseHandlerConfigurator(TaskScheduler taskScheduler, MessageHandler<TResponse> handler, Task requestTask)
        {
            _taskScheduler = taskScheduler;
            _handler = handler;
            _requestTask = requestTask;

            _specifications = new List<IPipeSpecification<ConsumeContext<TResponse>>>();
            _completed = new TaskCompletionSource<ConsumeContext<TResponse>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TResponse>> specification)
        {
            _specifications.Add(specification);
        }

        public HandlerConnectHandle<TResponse> Connect(IRequestPipeConnector connector, Guid requestId)
        {
            MessageHandler<TResponse> messageHandler = _handler != null ? (MessageHandler<TResponse>)AsyncMessageHandler : MessageHandler;

            var connectHandle = connector.ConnectRequestHandler(requestId, messageHandler, _specifications.ToArray());

            return new ResponseHandlerConnectHandle<TResponse>(connectHandle, _completed, _requestTask);
        }

        async Task AsyncMessageHandler(ConsumeContext<TResponse> context)
        {
            try
            {
                await Task.Factory.StartNew(() => _handler(context), context.CancellationToken, TaskCreationOptions.None, _taskScheduler)
                    .Unwrap()
                    .ConfigureAwait(false);

                _completed.TrySetResult(context);
            }
            catch (Exception ex)
            {
                _completed.TrySetException(ex);
            }
        }

        Task MessageHandler(ConsumeContext<TResponse> context)
        {
            try
            {
                _completed.TrySetResult(context);
            }
            catch (Exception ex)
            {
                _completed.TrySetException(ex);
            }

            return TaskUtil.Completed;
        }
    }
}