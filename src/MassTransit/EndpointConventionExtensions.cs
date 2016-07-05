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
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public static class EndpointConventionExtensions
    {
        public static async Task Send<T>(this ISendEndpointProvider provider, T message, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            Uri destinationAddress;
            if (!EndpointConvention.TryGetDestinationAddress(message, out destinationAddress))
            {
                throw new ArgumentException($"A convention for the message type {TypeMetadataCache<T>.ShortName} was not found");
            }

            var endpoint = await provider.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

            await endpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }
    }
}