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
    using GreenPipes;


    /// <summary>
    /// Connect a request pipe to the pipeline
    /// </summary>
    public interface IRequestPipeConnector
    {
        /// <summary>
        /// Connect the consume pipe to the pipeline for messages with the specified RequestId header
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestId"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class;
    }


    /// <summary>
    /// A connector for a pipe by request id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequestPipeConnector<out T>
        where T : class
    {
        /// <summary>
        /// Connect the consume pipe to the pipeline for messages with the specified RequestId header
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestPipe(Guid requestId, IPipe<ConsumeContext<T>> pipe);
    }
}