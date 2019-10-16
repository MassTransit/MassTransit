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
namespace MassTransit.Pipeline.Pipes
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Util;


    /// <summary>
    /// Merges the out-of-band consumer back into the context
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumerMessageMergePipe<TConsumer, TMessage> :
        IPipe<ConsumeContext<TMessage>>
        where TMessage : class
        where TConsumer : class
    {
        readonly ConsumerConsumeContext<TConsumer, TMessage> _context;
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _output;

        public ConsumerMessageMergePipe(IPipe<ConsumerConsumeContext<TConsumer, TMessage>> output, ConsumerConsumeContext<TConsumer, TMessage> context)
        {
            _output = output;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                ConsumerType = TypeMetadataCache<TConsumer>.ShortName,
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(ConsumeContext<TMessage> context)
        {
            if (ReferenceEquals(context, _context))
                return _output.Send(_context);

            return context is ConsumerConsumeContext<TConsumer, TMessage> consumerContext
                ? _output.Send(consumerContext)
                : _output.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, _context.Consumer));
        }
    }
}