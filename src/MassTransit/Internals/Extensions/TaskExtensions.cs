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
namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = TaskCompletionSourceFactory.New<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            return await task.ConfigureAwait(false);
        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = TaskCompletionSourceFactory.New<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            await task.ConfigureAwait(false);
        }

        public static async Task WithTimeout(this Task task, int milliseconds)
        {
            using (var tokenSource = new CancellationTokenSource(milliseconds))
            {
                var tcs = TaskCompletionSourceFactory.New<bool>();
                using (tokenSource.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                    if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                        throw new OperationCanceledException(tokenSource.Token);

                await task.ConfigureAwait(false);
            }
        }

        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            using (var tokenSource = new CancellationTokenSource(timeout))
            {
                var tcs = TaskCompletionSourceFactory.New<bool>();
                using (tokenSource.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                    if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                        throw new OperationCanceledException(tokenSource.Token);

                await task.ConfigureAwait(false);
            }
        }
    }
}
