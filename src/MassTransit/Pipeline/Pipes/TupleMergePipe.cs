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
namespace MassTransit.Pipeline.Pipes
{
    using System.Threading.Tasks;


    /// <summary>
    /// Merges the out-of-band consumer back into the pipe
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TupleMergePipe<TContext, TValue> :
        IPipe<TContext>
        where TContext : class, PipeContext
        where TValue : class
    {
        readonly TValue _value;
        readonly IPipe<TupleContext<TContext, TValue>> _output;

        public TupleMergePipe(TValue value, IPipe<TupleContext<TContext, TValue>> output)
        {
            _value = value;
            _output = output;
        }

        public Task Send(TContext context)
        {
            var tupleContext = new TupleContextProxy<TContext, TValue>(context, _value, context.CancellationToken);

            return _output.Send(tupleContext);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _output.Inspect(x));
        }
    }
}