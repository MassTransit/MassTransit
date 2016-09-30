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
namespace MassTransit.Steward
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;


    public class DispatchConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public DispatchConsumerFactory(IConsumerFactory<TConsumer> consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        async Task IConsumerFactory<TConsumer>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
        {
            DateTime timestamp = DateTime.UtcNow;
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await _consumerFactory.Send(context, next);

                timer.Stop();

                context.NotifyResourceUsageCompleted(context.ReceiveContext.InputAddress, timestamp, timer.Elapsed);
            }
            catch (Exception ex)
            {
                context.NotifyResourceUsageFailed(context.ReceiveContext.InputAddress, timestamp, timer.Elapsed, ex);
                throw;
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateConsumerFactoryScope<TConsumer>("dispatch");

            _consumerFactory.Probe(scope);
        }
    }
}