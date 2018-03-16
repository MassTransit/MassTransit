// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    using System.Threading.Tasks;
    using GreenPipes;


    public struct ConsumeSendContextPipe<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IPipe<SendContext<T>> _pipe;

        public ConsumeSendContextPipe(ConsumeContext consumeContext)
        {
            _consumeContext = consumeContext;

            _pipe = default;
        }

        public ConsumeSendContextPipe(ConsumeContext consumeContext, IPipe<SendContext<T>> pipe)
        {
            _consumeContext = consumeContext;
            _pipe = pipe;
        }

        public ConsumeSendContextPipe(ConsumeContext consumeContext, IPipe<SendContext> pipe)
        {
            _consumeContext = consumeContext;
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe?.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            if (_pipe.IsNotEmpty())
                await _pipe.Send(context).ConfigureAwait(false);
        }
    }
}