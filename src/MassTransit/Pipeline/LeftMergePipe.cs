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
namespace MassTransit.Pipeline
{
    using System.Threading.Tasks;


    public class LeftMergePipe<T1, T> :
        IPipe<ConsumeContext<T>>
        where T : class
    {
        readonly T1 _item1;
        readonly IPipe<ConsumerConsumeContext<T1, T>> _output;

        public LeftMergePipe(T1 item1, IPipe<ConsumerConsumeContext<T1, T>> output)
        {
            _item1 = item1;
            _output = output;
        }

        public Task Send(ConsumeContext<T> context)
        {
            return _output.Send(context.PushConsumer(_item1));
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _output.Inspect(x));
        }
    }
}