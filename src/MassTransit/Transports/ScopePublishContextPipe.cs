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
namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using Pipeline;


    public class ScopePublishContextPipe<T> :
        IPipe<PublishContext<T>>
        where T : class
    {
        readonly ConsumeContext _context;
        readonly IPipe<PublishContext<T>> _pipe;

        public ScopePublishContextPipe(IPipe<PublishContext<T>> pipe, ConsumeContext context)
        {
            _pipe = pipe;
            _context = context;
        }

        public ScopePublishContextPipe(IPipe<PublishContext> pipe, ConsumeContext context)
        {
            _pipe = pipe;
            _context = context;
        }

        public ScopePublishContextPipe(ConsumeContext context)
        {
            _pipe = Pipe.Empty<PublishContext<T>>();
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public Task Send(PublishContext<T> context)
        {
            if (_context.ConversationId.HasValue)
                context.ConversationId = _context.ConversationId;

            if (_context.CorrelationId.HasValue)
                context.InitiatorId = _context.CorrelationId;

            return _pipe.Send(context);
        }
    }
}