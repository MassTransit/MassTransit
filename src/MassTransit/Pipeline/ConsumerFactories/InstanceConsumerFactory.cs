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
namespace MassTransit.Pipeline.ConsumerFactories
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Retains a reference to an existing message consumer, and uses it to send consumable messages for
    /// processing.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class InstanceConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly TConsumer _consumer;

        public InstanceConsumerFactory(TConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return next.Send(context.PushConsumer(_consumer));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("instance");
        }
    }
}