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
    using Context;
    using Pipeline;


    public class PublishPipeContextAdapter :
        IPipe<SendContext>
    {
        readonly IPipe<PublishContext> _pipe;

        public PublishPipeContextAdapter(IPipe<PublishContext> pipe)
        {
            _pipe = pipe;
        }

        public Task Send(SendContext context)
        {
            var publishContext = new PublishContextProxy(context);

            return _pipe.Send(publishContext);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _pipe.Visit(x));
        }
    }


    public class PublishPipeContextAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<PublishContext<T>> _pipe;

        public PublishPipeContextAdapter(IPipe<PublishContext<T>> pipe)
        {
            _pipe = pipe;
        }

        public Task Send(SendContext<T> context)
        {
            var publishContext = new PublishContextProxy<T>(context);

            return _pipe.Send(publishContext);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _pipe.Visit(x));
        }
    }
}