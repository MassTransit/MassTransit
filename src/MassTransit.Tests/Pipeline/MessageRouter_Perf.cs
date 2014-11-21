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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Sinks;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TestFramework;


    [TestFixture]
    public class MessageRouter_Perf
    {
        [Test, Explicit]
        public void How_many_messages_can_the_pipe_send_per_second()
        {
            var consumer = new PerformantConsumer<PingMessage>(1500000);
            var factory = new DelegateConsumerFactory<PerformantConsumer<PingMessage>>(() => consumer);
            var sink = new ConsumerMessageSink<PerformantConsumer<PingMessage>, PingMessage>(factory);
            var router = new MessageRouter<IConsumeContext<PingMessage>>();
            router.Connect(sink);
            var translater = new InboundConvertMessageSink<PingMessage>(router);
            var objectRouter = new MessageRouter<IConsumeContext>();
            objectRouter.Connect(translater);
            var pipeline = new InboundMessagePipeline(objectRouter,
                MockRepository.GenerateMock<IInboundPipelineConfigurator>());

            var message = new PingMessage();
            IConsumeContext context = new OldConsumeContext<PingMessage>(OldReceiveContext.Empty(), message);

            for (int i = 0; i < 100; i++)
                pipeline.Dispatch(context);
            consumer.Reset();

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1500000; i++)
                pipeline.Dispatch(context);

            timer.Stop();

            Trace.WriteLine("Received: " + (consumer.Count) + ", expected " + 1500000);
            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + 1500000 * 1000 / timer.ElapsedMilliseconds);
        }

        [Test, Explicit]
        public void Nested_router_performance_measurement()
        {
            MessageRouter<IConsumeContext> router = SetupTwoRoutersOnly();

            const int primeLoopCount = 10;
            SendMessages(router, primeLoopCount);

            Stopwatch timer = Stopwatch.StartNew();

            const int loopCount = 1500000;
            SendMessages(router, loopCount);

            timer.Stop();

            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + loopCount * 1000 / timer.ElapsedMilliseconds);
        }

        [Test, Explicit]
        public void Router_performance_measurement()
        {
            MessageRouter<IConsumeContext<PingMessage>> router = SetupRouterOnly();

            const int primeLoopCount = 10;
            SendMessages(router, primeLoopCount);

            Stopwatch timer = Stopwatch.StartNew();

            const int loopCount = 1500000;
            SendMessages(router, loopCount);

            timer.Stop();

            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + loopCount * 1000 / timer.ElapsedMilliseconds);
        }

        [Test, Explicit]
        public async void TeeMessagePipe_performance()
        {
            var consumer = new TestConsumer<PingMessage>(1500000);

            IInboundPipe inboundPipe = new InboundPipe();
            ConnectHandle connectHandle = inboundPipe.ConnectInstance(consumer);

            var message = new PingMessage();
            var context = new TestConsumeContext<PingMessage>(message);

            for (int i = 0; i < 10; i++)
                await inboundPipe.Send(context);

            consumer.Reset();

            Stopwatch timer = Stopwatch.StartNew();

            const int loopCount = 1500000;
            var tasks = new Task[10];
            for (int loop = 0; loop < 10; loop++)
            {
                tasks[loop] = Task.Run(async () =>
                {
                    for (int i = 0; i < loopCount / 10; i++)
                        await inboundPipe.Send(context);
                });
            }

            await Task.WhenAll(tasks);

            await consumer.Task;

            timer.Stop();

            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + loopCount * 1000 / timer.ElapsedMilliseconds);
        }

        static PerformantConsumer<PingMessage> _consumer;


        internal class PerformantConsumer<T> :
            Consumes<T>.All
            where T : class
        {
            readonly ManualResetEvent _complete = new ManualResetEvent(false);
            readonly long _limit;
            long _count;

            public PerformantConsumer(long limit)
            {
                _limit = limit;
            }

            public ManualResetEvent Complete
            {
                get { return _complete; }
            }

            public long Count
            {
                get { return _count; }
            }

            public void Consume(T message)
            {
                long value = Interlocked.Increment(ref _count);
                if (value == _limit)
                    _complete.Set();
            }

            public void Reset()
            {
                _count = 0;
            }
        }


        class TestConsumer<T> :
            IConsumer<T>
            where T : class
        {
            readonly TaskCompletionSource<long> _completed;
            readonly long _limit;
            long _count;


            public TestConsumer(long limit)
            {
                _limit = limit;
                _completed = new TaskCompletionSource<long>();
            }

            public Task<long> Task
            {
                get { return _completed.Task; }
            }

            public async Task Consume(ConsumeContext<T> context)
            {
                long value = Interlocked.Increment(ref _count);
                if (value == _limit)
                    _completed.TrySetResult(value);
            }

            public void Reset()
            {
                _count = 0;
            }
        }


        class TestReceiveContext :
            ReceiveContext
        {
            PayloadCache _payloadCache;

            public TestReceiveContext()
            {
                _payloadCache = new PayloadCache();
            }

            public Encoding ContentEncoding { get; private set; }
            public CancellationToken CancellationToken { get; private set; }
            public Stream Body { get; private set; }
            public TimeSpan ElapsedTime { get; private set; }
            public Uri InputAddress { get; private set; }
            public ContentType ContentType { get; private set; }
            public bool Redelivered { get; private set; }
            public ContextHeaders TransportHeaders { get; private set; }

            public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
            {
            }

            public void NotifyFaulted(string messageType, string consumerType, Exception exception)
            {
            }

            public bool HasPayloadType(Type contextType)
            {
                return _payloadCache.HasPayloadType(contextType);
            }

            public bool TryGetPayload<TPayload>(out TPayload context)
                where TPayload : class
            {
                return _payloadCache.TryGetPayload(out context);
            }

            public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
                where TPayload : class
            {
                return _payloadCache.GetOrAddPayload(payloadFactory);
            }
        }


        static MessageRouter<IConsumeContext<PingMessage>> SetupRouterOnly()
        {
            _consumer = new PerformantConsumer<PingMessage>(1500000);

            var messageSink = new InstanceMessageSink<PingMessage>(MultipleHandlerSelector.ForHandler(
                HandlerSelector.ForHandler<PingMessage>(_consumer.Consume)));

            var router = new MessageRouter<IConsumeContext<PingMessage>>();
            router.Connect(messageSink);
            return router;
        }

        static MessageRouter<IConsumeContext> SetupTwoRoutersOnly()
        {
            _consumer = new PerformantConsumer<PingMessage>(1500000);

            var messageSink = new InstanceMessageSink<PingMessage>(MultipleHandlerSelector.ForHandler(
                HandlerSelector.ForHandler<PingMessage>(_consumer.Consume)));

            var router = new MessageRouter<IConsumeContext<PingMessage>>();
            router.Connect(messageSink);

            var translater = new InboundConvertMessageSink<PingMessage>(router);

            var nextRouter = new MessageRouter<IConsumeContext>();
            nextRouter.Connect(translater);

            return nextRouter;
        }

        static void SendMessages(IPipelineSink<IConsumeContext<PingMessage>> sink, int primeLoopCount)
        {
            var message = new PingMessage();
            var context = new OldConsumeContext<PingMessage>(OldReceiveContext.Empty(), message);

            for (int i = 0; i < primeLoopCount; i++)
            {
                foreach (var item in sink.Enumerate(context))
                    item(context);
            }
        }

        static void SendMessages(IPipelineSink<IConsumeContext> sink, int primeLoopCount)
        {
            var message = new PingMessage();
            var context = new OldConsumeContext<PingMessage>(OldReceiveContext.Empty(), message);

            for (int i = 0; i < primeLoopCount; i++)
            {
                foreach (var item in sink.Enumerate(context))
                    item(context);
            }
        }
    }


    [TestFixture]
    public class Throughput_Specs
    {
        [Test, Explicit]
        public void How_many_messages_can_the_pipe_send_per_second()
        {
            long count = 0;
            long count2 = 0;
            long limit = 2500000;

            var messageSink = new InstanceMessageSink<ClaimModified>(MultipleHandlerSelector.ForHandler(
                HandlerSelector.ForHandler<ClaimModified>(m => { Interlocked.Increment(ref count); })));

            var messageSink2 = new InstanceMessageSink<ClaimModified>(MultipleHandlerSelector.ForHandler(
                HandlerSelector.ForHandler<ClaimModified>(m => { count2++; })));

            var router = new MessageRouter<IConsumeContext<ClaimModified>>();
            router.Connect(messageSink);
            router.Connect(messageSink2);


            var translater = new InboundConvertMessageSink<ClaimModified>(router);
            var objectRouter = new MessageRouter<IConsumeContext>();
            objectRouter.Connect(translater);
            var pipeline = new InboundMessagePipeline(objectRouter,
                MockRepository.GenerateMock<IInboundPipelineConfigurator>());

            var message = new ClaimModified();
            var context = new OldConsumeContext<ClaimModified>(OldReceiveContext.Empty(), message);

            for (int i = 0; i < 100; i++)
                pipeline.Dispatch(context);

            count = 0;
            count2 = 0;

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < limit; i++)
                pipeline.Dispatch(context);

            timer.Stop();

            Trace.WriteLine("Received: " + (count + count2) + ", expected " + limit * 2);
            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + limit * 1000 / timer.ElapsedMilliseconds);
        }
    }


    public class ClaimModified
    {
    }
}