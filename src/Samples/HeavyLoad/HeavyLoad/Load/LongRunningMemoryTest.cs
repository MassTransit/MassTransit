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
namespace HeavyLoad.Load
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Correlated;
    using MassTransit;

    public class LongRunningMemoryTest :
        IDisposable
    {
        readonly IServiceBus _bus;

        int _requestCounter;
        int _responseCounter;

        public LongRunningMemoryTest()
        {
            _bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("msmq://localhost/heavy_load");
                    x.SetPurgeOnStartup(true);

                    x.UseMsmq();
                    x.UseMulticastSubscriptionClient();

                    x.Subscribe(s => s.Handler<SimpleRequest>(Handle));
                });
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Run(StopWatch stopWatch)
        {
            stopWatch.Start();

            Console.WriteLine("Starting long term memory test. Press any key to exit.");

            while (true)
            {
                for (int index = 0; index < 1000; index++)
                {
                    _bus.PublishRequest(new SimpleRequest(new Dictionary<string, string> {{"Name", "Value"}}), x =>
                        {
                            Interlocked.Increment(ref _requestCounter);

                            x.Handle<SimpleResponse>(message => { Interlocked.Increment(ref _responseCounter); });
                        });
                }

                DisplayMemoryUsage();

                if (Console.KeyAvailable)
                {
                    break;
                }
            }

            Console.WriteLine("Exiting...");
        }

        void DisplayMemoryUsage()
        {
            GC.Collect();

            var process = Process.GetCurrentProcess();

            var memorySize = process.PrivateMemorySize64;
            Console.WriteLine("Request Count: {0}, Memory Used: {1}", _requestCounter, memorySize);
        }

        void Handle(IConsumeContext<SimpleRequest> context, SimpleRequest message)
        {
            context.Respond(new SimpleResponse(message.Headers));
        }

        class SimpleRequest
        {
            public SimpleRequest(IDictionary<string, string> headers)
            {
                Headers = headers;
            }

            public SimpleRequest()
            {
            }

            public IDictionary<string, string> Headers { get; private set; }
        }

        class SimpleResponse
        {
            public SimpleResponse(IDictionary<string, string> headers)
            {
                Headers = headers;
            }

            public SimpleResponse()
            {
            }

            public IDictionary<string, string> Headers { get; private set; }
        }
    }
}
