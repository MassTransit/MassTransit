namespace MassTransit
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
    /// <typeparam name="TResult"></typeparam>
    public class ResultHandlerConfigurator<TResult> :
        IHandlerConfigurator<TResult>
        where TResult : class
    {
        readonly MessageHandler<TResult> _handler;
        readonly Task _requestTask;
        readonly IList<IPipeSpecification<ConsumeContext<TResult>>> _specifications;
        readonly TaskCompletionSource<ConsumeContext<TResult>> _completed;
        readonly TaskScheduler _taskScheduler;

        public ResultHandlerConfigurator(TaskScheduler taskScheduler, MessageHandler<TResult> handler, Task requestTask)
        {
            _taskScheduler = taskScheduler;
            _handler = handler;
            _requestTask = requestTask;

            _specifications = new List<IPipeSpecification<ConsumeContext<TResult>>>();
            _completed = new TaskCompletionSource<ConsumeContext<TResult>>();
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext<TResult>> specification)
        {
            _specifications.Add(specification);
        }

        public HandlerConnectHandle<TResult> Connect(IRequestPipeConnector connector, Guid requestId)
        {
            MessageHandler<TResult> messageHandler = _handler != null ? (MessageHandler<TResult>)AsyncMessageHandler : MessageHandler;

            var connectHandle = connector.ConnectRequestHandler(requestId, messageHandler, _specifications.ToArray());

            return new ResultHandlerConnectHandle<TResult>(connectHandle, _completed, _requestTask);
        }

        async Task AsyncMessageHandler(ConsumeContext<TResult> context)
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

        Task MessageHandler(ConsumeContext<TResult> context)
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