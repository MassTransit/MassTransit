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
    using Clients;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Microsoft.Owin;


    public class HttpResponseReceiveEndpointTopology :
        IReceiveEndpointTopology
    {
        readonly IOwinContext _owinContext;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly IMessageSerializer _serializer;
        readonly IReceiveEndpointTopology _topology;

        public HttpResponseReceiveEndpointTopology(IReceiveEndpointTopology topology, IOwinContext owinContext, ISendPipe sendPipe,
            IMessageSerializer serializer)
        {
            _topology = topology;
            _owinContext = owinContext;
            _sendPipe = sendPipe;
            _serializer = serializer;

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
        }

        Uri IReceiveEndpointTopology.InputAddress => _topology.InputAddress;

        ISendTopology IReceiveTopology.Send => _topology.Send;
        IPublishTopology IReceiveTopology.Publish => _topology.Publish;

        ISendEndpointProvider IReceiveTopology.SendEndpointProvider => _sendEndpointProvider.Value;
        IPublishEndpointProvider IReceiveTopology.PublishEndpointProvider => _topology.PublishEndpointProvider;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _topology.ConnectSendObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _topology.ConnectPublishObserver(observer);
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new HttpResponseSendEndpointProvider(_owinContext, _topology.InputAddress, _sendPipe, _serializer, _topology.SendEndpointProvider);
        }
    }
}