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
namespace MassTransit
{
    using System;


    public interface ConsumeContext :
        MessageContext
    {
        ReceiveContext ReceiveContext { get; }

        /// <summary>
        ///     Returns true if the specified message type is contained in the serialized message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool HasMessageType(Type messageType);

        /// <summary>
        ///     Returns the specified message type if available, otherwise returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumeContext"></param>
        /// <returns></returns>
        bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class;
    }


    public interface ConsumeContext<out T> :
        ConsumeContext
        where T : class
    {
        T Message { get; }
    }
}