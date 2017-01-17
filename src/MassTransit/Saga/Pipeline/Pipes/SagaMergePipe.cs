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
namespace MassTransit.Saga.Pipeline.Pipes
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Merges the out-of-band message back into the pipe
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMergePipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _output;

        public SagaMergePipe(IPipe<SagaConsumeContext<TSaga, TMessage>> output)
        {
            _output = output;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("merge");
            scope.Set(new
            {
                SagaType = TypeMetadataCache<TSaga>.ShortName,
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _output.Probe(scope);
        }

        public Task Send(SagaConsumeContext<TSaga> context)
        {
            return _output.Send(context.PopContext<TMessage>());
        }
    }
}