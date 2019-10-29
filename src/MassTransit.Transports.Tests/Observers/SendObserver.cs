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
namespace MassTransit.Transports.Tests.Observers
{
    using System;
    using System.Threading.Tasks;
    using Internals.Extensions;


    public class SendObserver :
        ISendObserver
    {
        readonly TaskCompletionSource<SendContext> _postSend = TaskCompletionSourceFactory.New<SendContext>();
        readonly TaskCompletionSource<SendContext> _preSend = TaskCompletionSourceFactory.New<SendContext>();
        readonly TaskCompletionSource<SendContext> _sendFaulted = TaskCompletionSourceFactory.New<SendContext>();

        public Task<SendContext> PreSent => _preSend.Task;

        public Task<SendContext> PostSent => _postSend.Task;

        public Task<SendContext> SendFaulted => _sendFaulted.Task;

        public async Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            _preSend.TrySetResult(context);
        }

        public async Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            _postSend.TrySetResult(context);
        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            _sendFaulted.TrySetResult(context);
        }
    }
}
