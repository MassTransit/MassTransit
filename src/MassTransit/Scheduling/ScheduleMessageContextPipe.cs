// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Scheduling
{
    using System.Threading.Tasks;
    using Pipeline;


    public class ScheduleMessageContextPipe<T> :
        IPipe<SendContext<ScheduleMessage<T>>>
        where T : class
    {
        readonly IPipe<SendContext<T>> _pipe;
        readonly IPipe<SendContext> _sendPipe;

        public ScheduleMessageContextPipe(IPipe<SendContext<T>> pipe)
        {
            _pipe = pipe;
        }

        public ScheduleMessageContextPipe(IPipe<SendContext> pipe)
        {
            _sendPipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
            _sendPipe?.Probe(context);
        }

        public async Task Send(SendContext<ScheduleMessage<T>> context)
        {
            if (_pipe != null)
            {
                SendContext<T> contextProxy = context.CreateProxy(context.Message.Payload);

                await _pipe.Send(contextProxy).ConfigureAwait(false);
            }
            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }
    }
}