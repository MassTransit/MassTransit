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
namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Microsoft.Owin;
    using Transport;
    using Transports;


    public class HttpResponseSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly Uri _inputAddress;
        readonly IOwinContext _owinContext;
        readonly ReceiveSettings _receiveSettings;
        readonly SendObservable _observers;
        readonly ISendPipe _sendPipe;

        public HttpResponseSendEndpointProvider(ReceiveSettings receiveSettings,
            IOwinContext owinContext,
            Uri inputAddress,
            ISendPipe sendPipe)
        {
            _receiveSettings = receiveSettings;
            _owinContext = owinContext;
            _inputAddress = inputAddress;
            _sendPipe = sendPipe;

            _observers = new SendObservable();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            if (address.Scheme == "reply")
            {
                var responseTransport = new HttpResponseTransport(_owinContext);

                var endpoint = new SendEndpoint(responseTransport, _receiveSettings.MessageSerializer, address, _inputAddress, _sendPipe);

                endpoint.ConnectSendObserver(_observers);

                return Task.FromResult<ISendEndpoint>(endpoint);
            }

            return _receiveSettings.SendEndpointProvider.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}