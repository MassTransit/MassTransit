// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// A factory that creates an execute activity and thenn invokes the pipe for the activity context
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public interface ExecuteActivityFactory<out TActivity, TArguments> :
        IProbeSite
        where TArguments : class
        where TActivity : class, ExecuteActivity<TArguments>
    {
        /// <summary>
        /// Executes the activity context by passing it to the activity factory, which creates the activity
        /// and then invokes the next pipe with the combined activity context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task<ResultContext<ExecutionResult>> Execute(ExecuteContext<TArguments> context, IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> next);
    }
}