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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Util;


    public class TransportReadyFilter<T> :
        Agent,
        IFilter<T>
        where T : class, PipeContext

    {
        readonly Uri _inputAddress;
        readonly IReceiveTransportObserver _transportObserver;

        public TransportReadyFilter(IReceiveTransportObserver transportObserver, Uri inputAddress)
        {
            _inputAddress = inputAddress;
            _transportObserver = transportObserver;
        }

        public async Task Send(T context, IPipe<T> next)
        {
            await _transportObserver.Ready(new ReceiveTransportReadyEvent(_inputAddress)).ConfigureAwait(false);

            SetReady();

            await next.Send(context).ConfigureAwait(false);

            SetCompleted(TaskUtil.Completed);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("transportReady");
        }
    }
}