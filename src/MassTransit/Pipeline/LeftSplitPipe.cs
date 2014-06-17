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

    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class LeftSplitPipe<T1, T> :
        IPipe<ConsumeContext<T1, T>>
        where T : class
    {
        readonly IFilter<ConsumeContext<T>> _next;
        readonly IPipe<ConsumeContext<T1, T>> _output;

        public LeftSplitPipe(IPipe<ConsumeContext<T1, T>> output, IFilter<ConsumeContext<T>> next)
        {
            _next = next;
            _output = output;
        }

        public Task Send(ConsumeContext<T1, T> context)
        {
            var output = new LeftMergePipe<T1, T>(context.Item1, _output);

            return _next.Send(context.Pop(), output);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _next.Inspect(x) && _output.Inspect(x));
        }
    }
}