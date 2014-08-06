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
    using System;
    using System.Threading.Tasks;


    public class LeftMergePipe<TLeft, T> :
        IPipe<ConsumeContext<T>>
        where T : class
    {
        readonly TLeft _left;
        readonly IPipe<ConsumeContext<Tuple<TLeft, ConsumeContext<T>>>> _output;

        public LeftMergePipe(TLeft left, IPipe<ConsumeContext<Tuple<TLeft, ConsumeContext<T>>>> output)
        {
            _left = left;
            _output = output;
        }

        public Task Send(ConsumeContext<T> context)
        {
            return _output.Send(context.PushLeft(_left));
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _output.Inspect(x));
        }
    }
}