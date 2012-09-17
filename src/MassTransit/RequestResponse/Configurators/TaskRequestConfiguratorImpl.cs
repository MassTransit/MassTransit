// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RequestResponse.Configurators
{
#if NET40
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TaskRequestConfiguratorImpl<TRequest> :
        RequestConfiguratorBase<TRequest>,
        TaskRequestConfigurator<TRequest>
        where TRequest : class
    {
        public TaskRequestConfiguratorImpl(TRequest message)
            : base(message)
        {
        }

        public Task<T> Handle<T>(Action<T> handler)
            where T : class
        {
            TaskResponseHandler<T> responseHandler = AddHandler(typeof(T),
                () => new CompleteTaskResponseHandler<T>(RequestId, handler));

            return responseHandler.Task;
        }

        public Task<T> Handle<T>(Action<IConsumeContext<T>, T> handler)
            where T : class
        {
            TaskResponseHandler<T> responseHandler = AddHandler(typeof(T),
                () => new CompleteTaskResponseHandler<T>(RequestId, handler));

            return responseHandler.Task;
        }

        public Task<Fault<TRequest>> HandleFault(Action<Fault<TRequest>> handler)
        {
            TaskResponseHandler<Fault<TRequest>> responseHandler = AddHandler(typeof(Fault<TRequest>),
                () => new CompleteTaskResponseHandler<Fault<TRequest>>(RequestId, handler));

            return responseHandler.Task;
        }

        public Task<Fault<TRequest>> HandleFault(Action<IConsumeContext<Fault<TRequest>>, Fault<TRequest>> handler)
        {
            TaskResponseHandler<Fault<TRequest>> responseHandler = AddHandler(typeof(Fault<TRequest>),
                () => new CompleteTaskResponseHandler<Fault<TRequest>>(RequestId, handler));

            return responseHandler.Task;
        }

        public void Watch<T>(Action<T> watcher)
            where T : class
        {
            AddHandler(typeof(T), () => new WatchTaskResponseHandler<T>(RequestId, watcher));
        }

        public void Watch<T>(Action<IConsumeContext<T>, T> watcher)
            where T : class
        {
            AddHandler(typeof(T), () => new WatchTaskResponseHandler<T>(RequestId, watcher));
        }

        public ITaskRequest<TRequest> Create(IServiceBus bus)
        {
            var request = new TaskRequest<TRequest>(RequestId, Request, Timeout, TimeoutHandler,
                CancellationToken.None, bus, Handlers);

            return request;
        }
    }
#endif
}