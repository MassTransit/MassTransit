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
namespace MassTransit.HttpTransport.Topology
{
    using System;
    using Clients;
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

        public Uri InputAddress => _topology.InputAddress;

        public ISendTopology Send => _topology.Send;
        public IPublishTopology Publish => _topology.Publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _topology.PublishEndpointProvider;

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new HttpResponseSendEndpointProvider(_owinContext, InputAddress, _sendPipe, _serializer, _topology.SendEndpointProvider);
        }
    }
}