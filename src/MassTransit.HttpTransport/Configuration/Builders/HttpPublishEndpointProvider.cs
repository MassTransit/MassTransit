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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public class HttpPublishEndpointProvider : IPublishEndpointProvider
    {
        readonly PublishObservable _publishObservable;

        public HttpPublishEndpointProvider()
        {
            _publishObservable = new PublishObservable();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId = null, Guid? conversationId = null)
        {
            return null;
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            return null;
        }
    }
}