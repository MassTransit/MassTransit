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
namespace MassTransit
{
    using System;
    using RequestResponse.Configurators;

#if NET40
    public static class TaskExtensions
    {
        public static ITaskRequest<TRequest> PublishRequestAsync<TRequest>(this IServiceBus bus, TRequest message,
            Action<TaskRequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            var configurator = new TaskRequestConfiguratorImpl<TRequest>(message);

            configureCallback(configurator);

            ITaskRequest<TRequest> request = configurator.Create(bus);

            bus.Publish(message, context => configurator.ApplyContext(context, bus.Endpoint.Address.Uri));

            return request;
        }
        
        public static ITaskRequest<TRequest> PublishRequestAsync<TRequest>(this IServiceBus bus, TRequest message,
          Action<TaskRequestConfigurator<TRequest>> configureCallback, Action<IPublishContext<TRequest>> contextConfigurator)
          where TRequest : class
        {
            var configurator = new TaskRequestConfiguratorImpl<TRequest>(message);

            configureCallback(configurator);

            ITaskRequest<TRequest> request = configurator.Create(bus);

            bus.Publish(message, context =>
                {
                    configurator.ApplyContext(context, bus.Endpoint.Address.Uri);
                    contextConfigurator(context);
                });

            return request;
        }

        public static ITaskRequest<TRequest> SendRequestAsync<TRequest>(this IEndpoint endpoint, IServiceBus bus,
            TRequest message,
            Action<TaskRequestConfigurator<TRequest>> configureCallback)
            where TRequest : class
        {
            var configurator = new TaskRequestConfiguratorImpl<TRequest>(message);

            configureCallback(configurator);

            ITaskRequest<TRequest> request = configurator.Create(bus);

            endpoint.Send(message, context => configurator.ApplyContext(context, bus.Endpoint.Address.Uri));

            return request;
        }
    }
#endif
}
