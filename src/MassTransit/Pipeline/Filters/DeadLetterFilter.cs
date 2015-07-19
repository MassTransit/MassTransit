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
    using Transports;


    /// <summary>
    /// Catches a pipeline exception and determines if the rescue pipe should be passed
    /// control of the context.
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
            await next.Send(context);

            if (context.IsDelivered || context.IsFaulted)
                return;

            context.InputAddress.LogSkipped(context.TransportHeaders.Get("MessageId", "N/A"));

            await _deadLetterPipe.Send(context);
        }
    }
}