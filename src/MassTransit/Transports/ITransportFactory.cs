// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Transports
{
    using System;

    public interface ITransportFactory :
        IDisposable
    {
        string Scheme { get; }

        IDuplexTransport BuildLoopback(ITransportSettings settings);
        IInboundTransport BuildInbound(ITransportSettings settings);
        IOutboundTransport BuildOutbound(ITransportSettings settings);
        IOutboundTransport BuildError(ITransportSettings settings);

        /// <summary>
        /// The message name formatter associated with this transport
        /// </summary>
        IMessageNameFormatter MessageNameFormatter { get; }

        IEndpointAddress GetAddress(Uri uri, bool transactional);
    }
}