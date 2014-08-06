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
    using NUnit.Framework;


    [TestFixture]
    public class AsyncTestFixture
    {
        CancellationTokenSource _testCancellationTokenSource;
        readonly TimeSpan _testTimeout;
        static TaskCompletionSource<bool> _cancelledTask;
        CancellationTokenRegistration _cancelledTaskTimeout;

        public AsyncTestFixture()
        {
            _testTimeout = Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(30);
            _cancelledTask = new TaskCompletionSource<bool>();
        }

        [TestFixtureSetUp]
        public void AsyncTestFixtureSetUp()
        {
            _testCancellationTokenSource = new CancellationTokenSource(_testTimeout);
            _cancelledTask = new TaskCompletionSource<bool>();

            _cancelledTaskTimeout = _testCancellationTokenSource.Token.Register(() => _cancelledTask.TrySetCanceled());
        }


        [TestFixtureTearDown]
        public void AsyncTestFixtureTearDown()
        {
            _cancelledTaskTimeout.Dispose();
            _testCancellationTokenSource.Dispose();
        }

        protected Task TestCancelledTask
        {
            get { return _cancelledTask.Task; }
        }

        protected CancellationToken TestCancellationToken
        {
            get { return _testCancellationTokenSource.Token; }
        }

        protected TimeSpan TestTimeout
        {
            get { return _testTimeout; }
        }

        protected Task<T> SubscribeToEvent<T>()
            where T : class
        {
            var source = new TaskCompletionSource<T>();

//            LocalBus.SubscribeHandler<T>(source.SetResult);

            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }


        protected Task TestHandler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
            where T : class
        {
            TaskCompletionSource<T> source = GetTaskCompletionSource<T>();

            configurator.Handler<T>(async context =>
            {
                try
                {
                    await handler(context);

                    source.TrySetResult(context.Message);
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);
                    throw;
                }
            });

            return source.Task;
        }

        protected Task TestHandler<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            TaskCompletionSource<T> source = GetTaskCompletionSource<T>();

            configurator.Handler<T>(async context =>
            {
                try
                {
                    source.TrySetResult(context.Message);
                }
                catch (Exception ex)
                {
                    source.TrySetException(ex);
                    throw;
                }
            });

            return source.Task;
        }

        TaskCompletionSource<T> GetTaskCompletionSource<T>() where T : class
        {
            var source = new TaskCompletionSource<T>();
            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            return source;
        }
    }
}