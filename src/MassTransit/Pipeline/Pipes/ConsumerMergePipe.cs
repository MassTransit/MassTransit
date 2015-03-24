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
namespace MassTransit.Pipeline.Pipes
{
    using System.Threading.Tasks;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerMergePipe<TConsumer, TMessage> :
        IPipe<ConsumerConsumeContext<TConsumer>>
        where TMessage : class
        where TConsumer : class
    {
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _output;

        public ConsumerMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output)
        {
            _output = output;
        }

        public Task Send(ConsumerConsumeContext<TConsumer> context)
        {
            return _output.Send(context.PopContext<TMessage>());
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _output.Visit(x));
        }
    }
}