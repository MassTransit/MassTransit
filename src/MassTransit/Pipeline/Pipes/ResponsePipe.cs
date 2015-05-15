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
namespace MassTransit.Pipeline.Pipes
{
    using System.Threading.Tasks;


    public class ResponsePipe<T> :
        IPipe<SendContext<T>>,
        IPipe<PublishContext<T>>
        where T : class
    {
        readonly ConsumeContext _context;
        readonly IPipe<SendContext<T>> _pipe;
        readonly IPipe<SendContext> _sendPipe;

        public ResponsePipe(ConsumeContext context)
        {
            _context = context;
        }

        public ResponsePipe(ConsumeContext context, IPipe<SendContext<T>> pipe)
        {
            _context = context;
            _pipe = pipe;
        }

        public ResponsePipe(ConsumeContext context, IPipe<SendContext> pipe)
        {
            _context = context;
            _sendPipe = pipe;
        }

        public Task Send(PublishContext<T> context)
        {
            SendContext<T> sendContext = context;

            return Send(sendContext);
        }

        public async Task Send(SendContext<T> context)
        {
            context.RequestId = _context.RequestId;
            context.SourceAddress = _context.ReceiveContext.InputAddress;

            if (_pipe != null)
                await _pipe.Send(context).ConfigureAwait(false);
            if (_sendPipe != null)
                await _sendPipe.Send(context).ConfigureAwait(false);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit<SendContext<T>>(this,
                x => (_pipe != null && _pipe.Visit(visitor)) || (_sendPipe != null && _sendPipe.Visit(visitor)));
        }
    }
}