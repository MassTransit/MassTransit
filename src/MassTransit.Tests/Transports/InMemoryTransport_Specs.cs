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
namespace MassTransit.Tests.Transports
{
    namespace InMemoryTransport_Specs
    {
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using MassTransit.Pipeline;
        using MassTransit.Serialization;
        using MassTransit.Transports;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Using_the_in_memory_transport :
            AsyncTestFixture
        {
            [Test]
            public async void Should_be_asynchronous()
            {
                var shutdown = new CancellationTokenSource();

                var inputAddress = new Uri("loopback://localhost/input_queue");

                var transport = new InMemoryTransport(inputAddress);

                TaskCompletionSource<int> received = GetTask<int>();

                IPipe<ReceiveContext> receivePipe = Pipe.New<ReceiveContext>(x =>
                {
                    x.Filter(new DelegateFilter<ReceiveContext>(context =>
                    {
                        Console.WriteLine("Message: {0}", context.TransportHeaders.Get("MessageId", "N/A"));

                        received.TrySetResult(1);
                    }));
                });

                Task receiveTask = ((IReceiveTransport)transport).Start(receivePipe, shutdown.Token);

                var sendEndpoint = new SendEndpoint(transport, new JsonSendMessageSerializer(JsonMessageSerializer.Serializer), inputAddress,
                    inputAddress);

                await sendEndpoint.Send(new A(), TestCancellationToken);

                await received.Task;

                shutdown.Cancel();

                await receiveTask;
            }
        }


        class A
        {
        }
    }
}