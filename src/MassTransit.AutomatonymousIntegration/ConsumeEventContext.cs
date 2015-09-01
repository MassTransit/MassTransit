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
namespace Automatonymous
{
    using MassTransit;


    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public interface ConsumeEventContext<out TInstance> :
        EventContext<TInstance>,
        ConsumeContext
    {
    }


    /// <summary>
    /// Combines the consumption of an event in a state machine with the consumer context of the receiving endpoint.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface ConsumeEventContext<out TInstance, out TData> :
        EventContext<TInstance, TData>,
        ConsumeContext
        where TData : class
    {
    }
}