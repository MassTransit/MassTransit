// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Topology
{
    using System;
    using Context;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Topology;
    using Microsoft.AspNetCore.Http;
    using Transport;


    public class HttpResponseReceiveEndpointContext :
        ReceiveEndpointContext
    {
        readonly HttpContext _httpContext;
        readonly ReceiveEndpointContext _receiveEndpointContext;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly IMessageSerializer _serializer;

        public HttpResponseReceiveEndpointContext(ReceiveEndpointContext receiveEndpointContext, HttpContext httpContext, ISendPipe sendPipe,
            IMessageSerializer serializer)
        {
            _receiveEndpointContext = receiveEndpointContext;
            _httpContext = httpContext;
            _sendPipe = sendPipe;
            _serializer = serializer;

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
        }

        Uri ReceiveEndpointContext.InputAddress => _receiveEndpointContext.InputAddress;
        ReceiveObservable ReceiveEndpointContext.ReceiveObservers => _receiveEndpointContext.ReceiveObservers;
        ReceiveTransportObservable ReceiveEndpointContext.TransportObservers => _receiveEndpointContext.TransportObservers;
        public ReceiveEndpointObservable EndpointObservers => _receiveEndpointContext.EndpointObservers;

        ISendTopology ReceiveEndpointContext.Send => _receiveEndpointContext.Send;
        IPublishTopology ReceiveEndpointContext.Publish => _receiveEndpointContext.Publish;

        ISendEndpointProvider ReceiveEndpointContext.SendEndpointProvider => _sendEndpointProvider.Value;
        IPublishEndpointProvider ReceiveEndpointContext.PublishEndpointProvider => _receiveEndpointContext.PublishEndpointProvider;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new HttpResponseSendEndpointProvider(_httpContext, _receiveEndpointContext.InputAddress, _sendPipe, _serializer,
                _receiveEndpointContext.SendEndpointProvider);
        }
    }
}