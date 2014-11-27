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
namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Pipes;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerSplitFilter<TConsumer, TMessage> :
        IFilter<ConsumerConsumeContext<TConsumer, TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IFilter<ConsumerConsumeContext<TConsumer>> _next;

        public ConsumerSplitFilter(IFilter<ConsumerConsumeContext<TConsumer>> next)
        {
            _next = next;
        }

        public Task Send(ConsumerConsumeContext<TConsumer, TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
        {
            var mergePipe = new ConsumerMergePipe<TConsumer, TMessage>(next);

            return _next.Send(context, mergePipe);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _next.Inspect(x) && _next.Inspect(x));
        }
    }
}