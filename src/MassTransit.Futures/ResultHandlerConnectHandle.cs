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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// A connection to a request which handles a result, and completes the Task when it's received
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ResultHandlerConnectHandle<TResult> :
        HandlerConnectHandle<TResult>
        where TResult : class
    {
        readonly ConnectHandle _handle;
        readonly TaskCompletionSource<ConsumeContext<TResult>> _completed;
        readonly Task _requestTask;

        public ResultHandlerConnectHandle(ConnectHandle handle, TaskCompletionSource<ConsumeContext<TResult>> completed, Task requestTask)
        {
            _handle = handle;
            _completed = completed;
            _requestTask = requestTask;

            Task = GetTask();
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public void Disconnect()
        {
            _handle.Disconnect();
        }

        public void TrySetException(Exception exception)
        {
            _completed.TrySetException(exception);
        }

        public void TrySetCanceled()
        {
            _completed.TrySetCanceled();
        }

        public Task<Result<TResult>> Task { get; }

        async Task<Result<TResult>> GetTask()
        {
            try
            {
                await _requestTask.ConfigureAwait(false);

                ConsumeContext<TResult> context = await _completed.Task.ConfigureAwait(false);

                return new MessageResult<TResult>(context);
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}