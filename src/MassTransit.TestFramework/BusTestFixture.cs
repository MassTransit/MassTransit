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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A bus text fixture includes a single bus instance with one or more receiving endpoints.
    /// </summary>
    public abstract class BusTestFixture :
        AsyncTestFixture
    {
        protected abstract IBus Bus { get; }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns>An awaitable task completed when the message is received</returns>
        protected Task<ConsumeContext<T>> SubscribeHandler<T>()
            where T : class
        {
            var source = new TaskCompletionSource<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                source.SetResult(context);

                handler.Disconnect();
            });

            TestCancelledTask.ContinueWith(x =>
            {
                source.TrySetCanceled();

                handler.Disconnect();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }

        /// <summary>
        /// Subscribes a message handler to the bus, which is disconnected after the message
        /// is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="filter">A filter that only completes the task if filter is true</param>
        /// <returns>An awaitable task completed when the message is received</returns>
        protected Task<ConsumeContext<T>> SubscribeHandler<T>(Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            var source = new TaskCompletionSource<ConsumeContext<T>>();

            ConnectHandle handler = null;
            handler = Bus.ConnectHandler<T>(async context =>
            {
                if (filter(context))
                {
                    source.SetResult(context);

                    handler.Disconnect();
                }
            });

            TestCancelledTask.ContinueWith(x =>
            {
                source.TrySetCanceled();

                handler.Disconnect();
            }, TaskContinuationOptions.OnlyOnCanceled);

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator)
            where T : class
        {
            var source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context => source.TrySetResult(context));

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="filter">Filter the messages based on the handled consume context</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, Func<ConsumeContext<T>, bool> filter)
            where T : class
        {
            var source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context =>
            {
                if (filter(context))
                    source.TrySetResult(context);
            });

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is cancelled when the test is canceled
        /// and completed when the message is received.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The endpoint configurator</param>
        /// <param name="expectedCount">The expected number of messages</param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handled<T>(IReceiveEndpointConfigurator configurator, int expectedCount)
            where T : class
        {
            var source = GetTask<ConsumeContext<T>>();

            int count = 0;
            configurator.Handler<T>(async context =>
            {
                var value = Interlocked.Increment(ref count);
                if(value == expectedCount)
                    source.TrySetResult(context);
            });

            return source.Task;
        }

        /// <summary>
        /// Registers a handler on the receive endpoint that is completed after the specified handler is 
        /// executed and canceled if the test is canceled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected Task<ConsumeContext<T>> Handler<T>(IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
            where T : class
        {
            var source = GetTask<ConsumeContext<T>>();

            configurator.Handler<T>(async context =>
            {
                await handler(context);
                source.TrySetResult(context);
            });

            return source.Task;
        }

        protected void LogEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.UseLog(Console.Out, (context,logContext) =>
                Task.FromResult($"Received (input_queue): {context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", context.SupportedMessageTypes)})"));
        }
    }
}