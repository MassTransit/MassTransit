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
namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;
    using Context;
    using Util;


    public class TestReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly IPublishObserver _publishObserver;

        public TestReceiveEndpointObserver(IPublishObserver publishObserver)
        {
            _publishObserver = publishObserver;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            LogContext.Debug?.Log("Endpoint Ready: {InputAddress}", ready.InputAddress);

            ready.ReceiveEndpoint.ConnectPublishObserver(_publishObserver);

            return TaskUtil.Completed;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return TaskUtil.Completed;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            LogContext.Debug?.Log("Endpoint Complete: {DeliveryCount}/{ConcurrentDeliveryCount} {InputAddress}", completed.DeliveryCount,
                completed.ConcurrentDeliveryCount, completed.InputAddress);

            return TaskUtil.Completed;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            LogContext.Debug?.Log("Endpoint Faulted: {Exception} {InputAddress}", faulted.Exception, faulted.InputAddress);

            return TaskUtil.Completed;
        }
    }
}
