// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context. 
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpoint
    {
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IReceiveTransport _receiveTransport;

        public ReceiveEndpoint(IReceiveTransport receiveTransport, IPipe<ReceiveContext> receivePipe)
        {
            _receiveTransport = receiveTransport;
            _receivePipe = receivePipe;
        }

        ReceiveEndpointHandle IReceiveEndpoint.Start()
        {
            ReceiveTransportHandle transportHandle = _receiveTransport.Start(_receivePipe);

            return new Handle(transportHandle);
        }

        public ObserverHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveTransport.ConnectReceiveObserver(observer);
        }


        class Handle :
            ReceiveEndpointHandle
        {
            readonly ReceiveTransportHandle _transportHandle;

            public Handle(ReceiveTransportHandle transportHandle)
            {
                _transportHandle = transportHandle;
            }

            void IDisposable.Dispose()
            {
                _transportHandle.Dispose();
            }

            async Task ReceiveEndpointHandle.Stop(CancellationToken cancellationToken)
            {
                await _transportHandle.Stop(cancellationToken);
            }
        }
    }
}