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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.IO;
    using Context;
    using MassTransit.Topology;


    public sealed class InMemoryReceiveContext :
        BaseReceiveContext
    {
        readonly byte[] _body;
        readonly InMemoryTransportMessage _message;

        public InMemoryReceiveContext(Uri inputAddress, InMemoryTransportMessage message, IReceiveObserver observer, IReceiveEndpointTopology topology)
            : base(inputAddress, message.DeliveryCount > 0, observer, topology)
        {
            _body = message.Body;
            _message = message;

            GetOrAddPayload(() => this);
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_message.Headers);

        protected override Stream GetBodyStream()
        {
            return new MemoryStream(_body, 0, _body.Length, false);
        }
    }
}