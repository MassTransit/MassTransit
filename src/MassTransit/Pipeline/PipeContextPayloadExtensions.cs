// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public static class PipeContextPayloadExtensions
    {
        /// <summary>
        /// Using a filter-supplied context type, block so that the one time code is only executed once regardless of how many
        /// threads are pushing through the pipe at the same time.
        /// </summary>
        /// <typeparam name="T">The payload type, should be an interface</typeparam>
        /// <param name="context">The pipe context</param>
        /// <param name="setupMethod">The setup method, called once regardless of the thread count</param>
        /// <param name="payloadFactory">The factory method for the payload context, optional if an interface is specified</param>
        /// <returns></returns>
        public static async Task OneTimeSetup<T>(this PipeContext context, Func<T, Task> setupMethod, PayloadFactory<T> payloadFactory = null)
            where T : class
        {
            OneTime<T> newContext = null;
            var existingContext = context.GetOrAddPayload<OneTimeSetupContext<T>>(() =>
            {
                var payload = payloadFactory?.Invoke() ?? TypeMetadataCache<T>.InitializeFromObject(new {});

                newContext = new OneTime<T>(payload);

                return newContext;
            });

            if (newContext == existingContext)
            {
                try
                {
                    await setupMethod(newContext.Payload).ConfigureAwait(false);

                    newContext.SetReady();
                }
                catch (Exception exception)
                {
                    newContext.SetFaulted(exception);

                    throw;
                }
            }
            else
                await existingContext.Ready.ConfigureAwait(false);
        }


        interface OneTimeSetupContext<T>
            where T : class
        {
            Task<T> Ready { get; }
        }


        class OneTime<T> :
            OneTimeSetupContext<T>
            where T : class
        {
            readonly TaskCompletionSource<T> _ready;

            public OneTime(T payload)
            {
                Payload = payload;
                _ready = new TaskCompletionSource<T>();
            }

            public T Payload { get; }

            public Task<T> Ready => _ready.Task;

            public void SetReady()
            {
                _ready.TrySetResult(Payload);
            }

            public void SetFaulted(Exception exception)
            {
                _ready.TrySetException(exception);
            }
        }
    }
}