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
namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Transports;


    /// <summary>
    /// If a message was neither delivered to a consumer nor caused a fault (which was notified already)
    /// then this filter will send the message to the dead letter pipe.
    /// </summary>
    public class DeadLetterFilter :
        IFilter<ReceiveContext>
    {
        readonly IPipe<ReceiveContext> _deadLetterPipe;

        public DeadLetterFilter(IPipe<ReceiveContext> deadLetterPipe)
        {
            _deadLetterPipe = deadLetterPipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("deadLetter");

            _deadLetterPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            await next.Send(context).ConfigureAwait(false);

            if (context.IsDelivered || context.IsFaulted)
                return;

            context.LogSkipped();

            await _deadLetterPipe.Send(context).ConfigureAwait(false);
        }
    }
}