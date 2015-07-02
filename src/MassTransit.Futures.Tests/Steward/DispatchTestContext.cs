// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Steward
{
    using System;
    using ConsumeConfigurators;
    using TestFramework;


    public interface DispatchTestContext
    {
        Uri ExecuteUri { get; }

        void Configure(ActivityTestContextConfigurator configurator);
    }


    public class DispatchTestContext<TConsumer, T> :
        DispatchTestContext
        where T : class
        where TConsumer : class, IConsumer<T>
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        Action<IConsumerConfigurator<TConsumer>> _configure;

        public DispatchTestContext(Uri baseUri, IConsumerFactory<TConsumer> consumerFactory, Action<IConsumerConfigurator<TConsumer>> configure)
        {
            _consumerFactory = consumerFactory;
            _configure = configure;

            Name = GetServiceName();
            ExecuteQueueName = BuildQueueName("execute");

            ExecuteUri = BuildQueueUri(baseUri, ExecuteQueueName);
        }

        public string Name { get; private set; }
        public string ExecuteQueueName { get; private set; }
        public Uri ExecuteUri { get; private set; }

        public void Configure(ActivityTestContextConfigurator configurator)
        {
            configurator.ReceiveEndpoint(ExecuteQueueName, x => x.Consumer(_consumerFactory, _configure));
        }

        Uri BuildQueueUri(Uri baseUri, string queueName)
        {
            return new Uri(baseUri, queueName);
        }

        string BuildQueueName(string prefix)
        {
            return string.Format("{0}_{1}", prefix, typeof(T).Name.ToLowerInvariant());
        }

        static string GetServiceName()
        {
            return typeof(T).Name + "Service";
        }
    }
}