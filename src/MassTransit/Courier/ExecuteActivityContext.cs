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
    public interface ExecuteActivityContext<out TArguments> :
        ExecuteContext<TArguments>
        where TArguments : class
    {
    }


    /// <summary>
    /// An activity and execution context combined into a single container from the factory
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TArguments"></typeparam>
    public interface ExecuteActivityContext<out TActivity, out TArguments> :
        ExecuteActivityContext<TArguments>
        where TArguments : class
        where TActivity : class, ExecuteActivity<TArguments>
    {
        /// <summary>
        /// The activity that was created/used for this execution
        /// </summary>
        TActivity Activity { get; }
    }
}