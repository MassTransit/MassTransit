// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport
{
    using System;
    using GreenPipes;
    using Hosting;
    using Topology;
    using Transport;
    using Util;


    public static class ConsumerPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a RabbitMQ Basic Consumer to the pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="receivePipe"></param>
        /// <param name="settings"></param>
        /// <param name="receiveSettings"></param>
        /// <param name="receiveObserver"></param>
        /// <param name="receiveTransportObserver"></param>
        /// <param name="supervisor"></param>
        /// <param name="topology"></param>
        public static void HttpConsumer(this IPipeConfigurator<OwinHostContext> configurator, IPipe<ReceiveContext> receivePipe, HttpHostSettings settings,
            ReceiveSettings receiveSettings, IReceiveObserver receiveObserver, IReceiveTransportObserver receiveTransportObserver, ITaskSupervisor supervisor, IHttpReceiveEndpointTopology topology)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new HttpConsumerPipeSpecification(settings, receiveSettings, receivePipe, receiveObserver, receiveTransportObserver,
                supervisor, topology);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}