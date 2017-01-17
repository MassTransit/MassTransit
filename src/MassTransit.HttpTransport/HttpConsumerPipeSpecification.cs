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
namespace MassTransit.HttpTransport
{
    using System.Collections.Generic;
    using GreenPipes;
    using Hosting;
    using Topology;
    using Transport;
    using Util;


    public class HttpConsumerPipeSpecification :
        IPipeSpecification<OwinHostContext>
    {
        readonly HttpHostSettings _hostSettings;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        readonly ITaskSupervisor _supervisor;
        readonly IHttpReceiveEndpointTopology _topology;
        readonly IReceiveTransportObserver _transportObserver;

        public HttpConsumerPipeSpecification(HttpHostSettings hostSettings,
            ReceiveSettings receiveSettings,
            IPipe<ReceiveContext> receivePipe,
            IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver,
            ITaskSupervisor supervisor, IHttpReceiveEndpointTopology topology)
        {
            _hostSettings = hostSettings;
            _receiveSettings = receiveSettings;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _topology = topology;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            builder.AddFilter(new HttpConsumerFilter(_receivePipe, _receiveObserver, _transportObserver, _supervisor, _hostSettings, _receiveSettings,
                _topology));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_hostSettings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}