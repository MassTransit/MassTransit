// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class DeserializeFilter :
        IFilter<ReceiveContext>
    {
        readonly IMessageDeserializer _deserializer;
        readonly IPipe<ConsumeContext> _output;

        public DeserializeFilter(IMessageDeserializer deserializer, IPipe<ConsumeContext> output)
        {
            _deserializer = deserializer;
            _output = output;
        }

        public async Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            ConsumeContext consumeContext = _deserializer.Deserialize(context);

//            acceptContext.SetEndpoint(this);

            await _output.Send(consumeContext);

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _output.Inspect(x));
        }
    }
}