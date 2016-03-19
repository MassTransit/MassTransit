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
namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// Calls the generic version of the ISendEndpoint.Send method with the object's type
    /// </summary>
    public interface IMessageSchedulerConverter
    {
        Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken);

        Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken);
    }
}