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
namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Transport;


    public class SubscriptionClientContext :
        BasePipeContext,
        ClientContext,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<QueueClientContext>();
        readonly IMessageReceiver _messageReceiver;
        readonly SubscriptionSettings _settings;

        public SubscriptionClientContext(IMessageReceiver messageReceiver, Uri inputAddress, SubscriptionSettings settings)
        {
            _messageReceiver = messageReceiver;
            _settings = settings;
            InputAddress = inputAddress;
        }

        public string EntityPath => _settings.TopicDescription.Path;
        public Uri InputAddress { get; }

        public void OnMessageAsync(Func<IMessageReceiver, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler)
        {
            _messageReceiver.RegisterMessageHandler(async (message, token) =>
            {
                await callback(_messageReceiver, message, token);
            }, _settings.GetOnMessageOptions(exceptionHandler));
        }

        async Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Closing client: {0}", InputAddress);

            try
            {
                if (_messageReceiver != null && !_messageReceiver.IsClosedOrClosing)
                    await _messageReceiver.CloseAsync().ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Closed client: {0}", InputAddress);
            }
            catch (Exception exception)
            {
                if (_log.IsWarnEnabled)
                    _log.Warn($"Exception closing the client: {InputAddress}", exception);
            }
        }
    }
}