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
namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Microsoft.Owin;
    using Transports;


    public class HttpResponseSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly Uri _inputAddress;
        readonly SendObservable _observers;
        readonly IOwinContext _owinContext;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly IMessageSerializer _serializer;

        public HttpResponseSendEndpointProvider(IOwinContext owinContext, Uri inputAddress, ISendPipe sendPipe, IMessageSerializer serializer,
            ISendEndpointProvider sendEndpointProvider)
        {
            _serializer = serializer;
            _owinContext = owinContext;
            _inputAddress = inputAddress;
            _sendPipe = sendPipe;

            _observers = new SendObservable();
            _sendEndpointProvider = sendEndpointProvider;
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            if (address.Scheme == "reply")
            {
                var responseTransport = new HttpResponseTransport(_owinContext);

                var endpoint = new SendEndpoint(responseTransport, _serializer, address, _inputAddress, _sendPipe);

                endpoint.ConnectSendObserver(_observers);

                return Task.FromResult<ISendEndpoint>(endpoint);
            }

            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}