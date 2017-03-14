// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;


    /// <summary>
    /// A request client factory which is unique to each consume, but uses a return path specified
    /// by the creation of the factory
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestClientFactory<TRequest, TResponse> :
        IAsyncDisposable
        where TRequest : class
        where TResponse : class
    {
        IRequestClient<TRequest, TResponse> CreateRequestClient(ISendEndpointProvider sendEndpointProvider, TimeSpan? timeout = null,
            TimeSpan? timeToLive = null, Action<SendContext<TRequest>> callback = null);
    }
}