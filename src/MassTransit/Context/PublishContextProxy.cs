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
namespace MassTransit.Context
{
    public class PublishContextProxy :
        SendContextProxy,
        PublishContext
    {
        readonly SendContext _context;

        public PublishContextProxy(SendContext context)
            : base(context)
        {
            _context = context;

            _context.GetOrAddPayload<PublishContext>(() => this);
        }

        bool PublishContext.Mandatory { get; set; }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new PublishContextProxy<T>(_context, message);
        }
    }


    public class PublishContextProxy<TMessage> :
        SendContextProxy<TMessage>,
        PublishContext<TMessage>
        where TMessage : class
    {
        readonly SendContext _context;

        public PublishContextProxy(SendContext context, TMessage message)
            : base(context, message)
        {
            _context = context;

            _context.GetOrAddPayload<PublishContext>(() => this);
            _context.GetOrAddPayload<PublishContext<TMessage>>(() => this);
        }

        bool PublishContext.Mandatory { get; set; }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new PublishContextProxy<T>(_context, message);
        }
    }
}