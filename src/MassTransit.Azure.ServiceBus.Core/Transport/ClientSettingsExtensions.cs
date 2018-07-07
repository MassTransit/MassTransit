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
namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;


    public static class ClientSettingsExtensions
    {
        public static SessionHandlerOptions GetSessionHandlerOptions(this ClientSettings settings, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var options = new SessionHandlerOptions(exceptionHandler)
            {
                AutoComplete = false,
                MaxAutoRenewDuration = settings.MaxAutoRenewDuration,
                MaxConcurrentSessions = settings.MaxConcurrentCalls,
                MessageWaitTimeout = settings.MessageWaitTimeout
            };

            return options;
        }

        public static MessageHandlerOptions GetOnMessageOptions(this ClientSettings settings, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            var options = new MessageHandlerOptions(exceptionHandler)
            {
                AutoComplete = false,
                MaxAutoRenewDuration = settings.MaxAutoRenewDuration,
                MaxConcurrentCalls = settings.MaxConcurrentCalls
            };

            return options;
        }
    }
}