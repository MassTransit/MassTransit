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
namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Pipeline.Observables;
    using Microsoft.AspNetCore.Http;
    using Transports;


    public class HttpResponseSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly SendObservable _observers;
        readonly HttpContext _httpContext;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        public HttpResponseSendEndpointProvider(HttpContext httpContext, ReceiveEndpointContext receiveEndpointContext)
        {
            _receiveEndpointContext = receiveEndpointContext;
            _httpContext = httpContext;

            _observers = new SendObservable();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            if (address.Scheme == "reply")
            {
                var responseTransport = new HttpResponseTransport(_httpContext);

                var endpoint = new SendEndpoint(responseTransport, _receiveEndpointContext.Serializer, address, _receiveEndpointContext.InputAddress,
                    _receiveEndpointContext.SendPipe);

                endpoint.ConnectSendObserver(_observers);

                return Task.FromResult<ISendEndpoint>(endpoint);
            }

            return _receiveEndpointContext.SendEndpointProvider.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}