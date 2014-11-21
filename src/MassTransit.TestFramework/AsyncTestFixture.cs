// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TestFramework
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using EndpointConfigurators;


    public abstract class AsyncTestFixture
    {
        CancellationToken _cancellationToken;
        Task<bool> _cancelledTask;
        CancellationTokenSource _cancellationTokenSource;

        protected AsyncTestFixture()
        {
            TestTimeout = Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(30);
        }

        protected Task TestCancelledTask
        {
            get
            {
                CancellationToken token = TestCancellationToken;
                return _cancelledTask;
            }
        }

        protected CancellationToken TestCancellationToken
        {
            get
            {
                if (_cancellationToken == CancellationToken.None)
                {
                    _cancellationTokenSource = new CancellationTokenSource((int)TestTimeout.TotalMilliseconds);
                    _cancellationToken = _cancellationTokenSource.Token;

                    var source = new TaskCompletionSource<bool>();
                    _cancelledTask = source.Task;

                    _cancellationToken.Register(() => source.TrySetCanceled());
                }

                return _cancellationToken;
            }
        }

        protected TimeSpan TestTimeout { get; set; }

        protected void CancelTest()
        {
            _cancellationTokenSource.Cancel();
        }


        protected TaskCompletionSource<T> GetTask<T>()
        {
            var source = new TaskCompletionSource<T>();

            TestCancellationToken.Register(() => source.TrySetCanceled());

            return source;
        }

        protected Task<T> Handler<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            var source = new TaskCompletionSource<T>();

            configurator.Handler<T>(async context => source.SetResult(context.Message));

            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }
    }
}