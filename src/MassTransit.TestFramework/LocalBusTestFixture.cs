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
    using System.Threading.Tasks;


    public abstract class LocalBusTestFixture :
        AsyncTestFixture
    {
        protected abstract IBus LocalBus { get; }

        protected Task<T> SubscribeToLocalBus<T>()
            where T : class
        {
            var source = new TaskCompletionSource<T>();

            ConnectHandle handler = null;
            handler = LocalBus.SubscribeHandler<T>(async context =>
            {
                source.SetResult(context.Message);

                handler.Disconnect();
            });

            TestCancelledTask.ContinueWith(x =>
            {
                source.TrySetCanceled();
                handler.Disconnect();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }

        protected Task<T> SubscribeToLocalBus<T>(Func<T, bool> handler)
            where T : class
        {
            var source = new TaskCompletionSource<T>();

            LocalBus.SubscribeHandler<T>(async context =>
            {
                if (handler(context.Message))
                    source.SetResult(context.Message);
            });

            TestCancelledTask.ContinueWith(x => source.TrySetCanceled(), TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }
    }
}