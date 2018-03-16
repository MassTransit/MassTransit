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


    public class BusClientFactoryContext :
        ClientFactoryContext
    {
        readonly IBus _bus;

        public BusClientFactoryContext(IBus bus, RequestTimeout defaultTimeout = default)
        {
            _bus = bus;

            DefaultTimeout = defaultTimeout.HasValue ? defaultTimeout : RequestTimeout.Default;
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _bus.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _bus.GetSendEndpoint(address);
        }

        public Uri ResponseAddress => _bus.Address;

        public IPublishEndpoint PublishEndpoint => _bus;

        public RequestTimeout DefaultTimeout { get; }
    }
}