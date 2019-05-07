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
namespace MassTransit.WebJobs.EventHubsIntegration.Contexts
{
    using System.Threading;
    using Context;
    using EventHubsIntegration;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;


    public class WebJobEventDataReceiveEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;
        readonly ILogger _logger;

        public WebJobEventDataReceiveEndpointContext(IReceiveEndpointConfiguration configuration, ILogger logger, IBinder binder,
            CancellationToken cancellationToken)
            : base(configuration)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;
            _logger = logger;

        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new EventHubAttributeSendTransportProvider(_binder, _logger, _cancellationToken);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new EventHubAttributePublishTransportProvider(SendTransportProvider);
        }
    }
}
