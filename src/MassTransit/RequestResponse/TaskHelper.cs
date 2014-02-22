// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskHelper
    {
        static readonly Task _canceled;
        static readonly Task _completed;

        static TaskHelper()
        {
            var source = new TaskCompletionSource<bool>();
            source.TrySetCanceled();

            _canceled = source.Task;
            _completed = FromResult(true);
        }

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var source = new TaskCompletionSource<TResult>(result);
            source.TrySetResult(result);
            return source.Task;
        }

        public static Task Timeout(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be >= -1");

            if (cancellationToken.IsCancellationRequested)
                return _canceled;

            if (timeout == TimeSpan.Zero)
                return _completed;

            var source = new TaskCompletionSource<bool>();
            var tokenRegistration = new CancellationTokenRegistration();

            var timer = new Timer(delegate(object self)
                {
                    tokenRegistration.Dispose();
                    ((Timer)self).Dispose();
                    source.TrySetResult(true);
                });

            if (cancellationToken.CanBeCanceled)
            {
                Action callback = () =>
                    {
                        timer.Dispose();
                        source.TrySetCanceled();
                    };

                tokenRegistration = cancellationToken.Register(callback);
            }

            timer.Change(timeout, TimeSpan.FromMilliseconds(-1));

            return source.Task;
        }

        public static Task Timeout(TimeSpan timeout)
        {
            return Timeout(timeout, CancellationToken.None);
        }
    }
}