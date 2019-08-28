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
namespace MassTransit.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Initializers;


    public class SendRequestSendEndpoint<T> :
        IRequestSendEndpoint<T>
        where T : class
    {
        readonly ISendEndpointProvider _context;
        readonly Uri _destinationAddress;

        public SendRequestSendEndpoint(ISendEndpointProvider context, Uri destinationAddress)
        {
            _context = context;
            _destinationAddress = destinationAddress;
        }

        public Task<InitializeContext<T>> CreateMessage(object values, CancellationToken cancellationToken)
        {
            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            return _context is ConsumeContext context
                ? initializer.Initialize(initializer.Create(context), values)
                : initializer.Initialize(values, cancellationToken);
        }

        public async Task Send(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _context.GetSendEndpoint(_destinationAddress).ConfigureAwait(false);

            await sendEndpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);
        }
    }
}
