// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum.Caching;

    public class MessageRetryTracker
    {
        readonly Cache<string, TrackedMessage> _messages;

        readonly int _retryLimit;

        public MessageRetryTracker(int retryLimit)
        {
            _retryLimit = retryLimit;

            _messages = new ConcurrentCache<string, TrackedMessage>(id => new TrackedMessage());
        }

        public bool IsRetryLimitExceeded(string id, out Exception retryException)
        {
            bool exceeded = false;
            Exception result = null;

            if (!string.IsNullOrEmpty(id))
            {
                _messages.WithValue(id, x =>
                    {
                        result = x.Exception;
                        exceeded = x.RetryCount >= _retryLimit;
                    });
            }

            retryException = result;
            return exceeded;
        }

        public void IncrementRetryCount(string id, Exception ex)
        {
            if (string.IsNullOrEmpty(id))
                return;

            _messages[id].Increment(ex);
        }

        public void MessageWasReceivedSuccessfully(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            _messages.Remove(id);
        }

        class TrackedMessage
        {
            public Exception Exception;
            public int RetryCount;

            public void Increment(Exception exception)
            {
                lock(this)
                {
                    RetryCount++;
                    Exception = exception;
                }
            }
        }
    }
}