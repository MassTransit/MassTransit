// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Testing.TestDecorators;
    using Util;


    public abstract class AsyncTestFixture
    {
        CancellationToken _cancellationToken;
        CancellationTokenSource _cancellationTokenSource;
        Task<bool> _cancelledTask;

        protected AsyncTestFixture()
        {
            TestTimeout = Debugger.IsAttached ? TimeSpan.FromMinutes(50) : TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Task that is canceled when the test is aborted, for continueWith usage
        /// </summary>
        protected Task TestCancelledTask
        {
            get
            {
                CancellationToken token = TestCancellationToken;
                return _cancelledTask;
            }
        }

        /// <summary>
        /// CancellationToken that is canceled when the test is being aborted
        /// </summary>
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

        /// <summary>
        /// Timeout for the test, used for any delay timers
        /// </summary>
        protected TimeSpan TestTimeout { get; set; }

        /// <summary>
        /// Forces the test to be cancelled, aborting any awaiting tasks
        /// </summary>
        protected void CancelTest()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Returns a task completion that is automatically canceled when the test is canceled
        /// </summary>
        /// <typeparam name="T">The task type</typeparam>
        /// <returns></returns>
        protected TaskCompletionSource<T> GetTask<T>()
        {
            var source = new TaskCompletionSource<T>();

            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source;
        }

        protected TestConsumeObserver<T> GetConsumeObserver<T>()
            where T : class
        {
            return new TestConsumeObserver<T>(GetTask<T>(), GetTask<T>(), GetTask<T>());
        }

        protected TestConsumeObserver GetConsumeObserver()
        {
            return new TestConsumeObserver(TestTimeout);
        }

        protected TestObserver<T> GetObserver<T>() 
            where T : class
        {
            return new TestObserver<T>(GetTask<ConsumeContext<T>>(), GetTask<Exception>(), GetTask<bool>());
        }

        protected TestSendObserver GetSendObserver()
        {
            return new TestSendObserver(TestTimeout);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        protected void Await(Func<Task> taskFactory)
        {
            TaskUtil.Await(taskFactory, CancellationToken.None);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="cancellationToken"></param>
        protected void Await(Func<Task> taskFactory, CancellationToken cancellationToken)
        {
            TaskUtil.Await(taskFactory, cancellationToken);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        protected T Await<T>(Func<Task<T>> taskFactory)
        {
            return TaskUtil.Await(taskFactory, TestCancellationToken);
        }

        /// <summary>
        /// Await a task in a test method that is not asynchronous, such as a test fixture setup
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="cancellationToken"></param>
        protected T Await<T>(Func<Task<T>> taskFactory, CancellationToken cancellationToken)
        {
            return TaskUtil.Await(taskFactory, cancellationToken);
        }
    }
}