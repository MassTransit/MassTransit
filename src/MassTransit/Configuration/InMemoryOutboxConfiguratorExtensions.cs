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
namespace MassTransit
{
    using System;
    using GreenPipes;
    using PipeConfigurators;


    public static class InMemoryOutboxConfiguratorExtensions
    {
        /// <summary>
        /// Includes an outbox in the consume filter path, which delays outgoing messages until the return path
        /// of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
        /// nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        public static void UseInMemoryOutbox(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var observer = new InMemoryOutboxConfigurationObserver(configurator);
        }

        /// <summary>
        /// Includes an outbox in the consume filter path, which delays outgoing messages until the return path
        /// of the pipeline returns to the outbox filter. At this point, the message execution pipeline should be
        /// nearly complete with only the ack remaining. If an exception is thrown, the messages are not sent/published.
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        public static void UseInMemoryOutbox<T>(this IPipeConfigurator<ConsumeContext<T>> configurator)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new InMemoryOutboxSpecification<T>();

            configurator.AddPipeSpecification(specification);
        }
    }
}