// Copyright 2007-2014 Chris Patterson
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


    public interface ExecuteActivity<in TArguments>
        where TArguments : class
    {
        /// <summary>
        /// Execute the activity
        /// </summary>
        /// <param name="execution">The execution context</param>
        /// <returns>An execution result, created from the execution passed to the activity</returns>
        Task<ExecutionResult> Execute(Execution<TArguments> execution);
    }
}