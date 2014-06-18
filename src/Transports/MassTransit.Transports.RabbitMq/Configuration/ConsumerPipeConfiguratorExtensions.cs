// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Pipeline;
    using Transports.RabbitMq;
    using Transports.RabbitMq.Configuration;


    public static class ConsumerPipeConfiguratorExtensions
    {
        /// <summary>
        /// Adds a RabbitMQ Basic Consumer to the pipeline
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="pipe"></param>
        /// <param name="settings"></param>
        public static void ModelConsumer(this IPipeConfigurator<ConnectionContext> configurator, IPipe<ReceiveContext> pipe,
            ReceiveSettings settings)
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            var pipeBuilderConfigurator = new ModelConsumerPipeBuilderConfigurator(pipe, settings);

            configurator.AddPipeBuilderConfigurator(pipeBuilderConfigurator);
        }
    }
}