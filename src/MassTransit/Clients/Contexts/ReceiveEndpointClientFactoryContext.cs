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
namespace MassTransit.Clients.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public class ReceiveEndpointClientFactoryContext :
        ClientFactoryContext
    {
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly IReceiveEndpoint _receiveEndpoint;

        public ReceiveEndpointClientFactoryContext(ReceiveEndpointReady receiveEndpointReady, RequestTimeout defaultTimeout = default)
        {
            _receiveEndpoint = receiveEndpointReady.ReceiveEndpoint;

            ResponseAddress = receiveEndpointReady.InputAddress;
            DefaultTimeout = defaultTimeout.Or(RequestTimeout.Default);

            _publishEndpoint = new Lazy<IPublishEndpoint>(CreatePublishEndpoint);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receiveEndpoint.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _receiveEndpoint.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpoint.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _receiveEndpoint.GetSendEndpoint(address);
        }

        public Uri ResponseAddress { get; }

        public IPublishEndpoint PublishEndpoint => _publishEndpoint.Value;

        public RequestTimeout DefaultTimeout { get; }

        IPublishEndpoint CreatePublishEndpoint()
        {
            return _receiveEndpoint.CreatePublishEndpoint(ResponseAddress);
        }
    }
}
