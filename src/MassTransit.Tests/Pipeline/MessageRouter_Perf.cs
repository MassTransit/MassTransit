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

	[TestFixture]
	public class MessageRouter_Perf
	{
		private static PerformantConsumer<PingMessage> _consumer;

		internal class PerformantConsumer<T> :
			Consumes<T>.All
			where T : class
		{
			private readonly long _limit;
			private long _count;
			private readonly ManualResetEvent _complete = new ManualResetEvent(false);

			public ManualResetEvent Complete
			{
				get { return _complete; }
			}

		    public long Count { get { return _count; }}

		    public PerformantConsumer(long limit)
			{
				_limit = limit;
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
	        readonly long _limit;
	        long _count;
	        readonly TaskCompletionSource<long> _completed; 


	        public TestConsumer(long limit)
	        {
	            _limit = limit;
                _completed = new TaskCompletionSource<long>();
	        }

	        public Task<long> Task
	        {
                get { return _completed.Task; }
	        }

	        public async Task Consume(MassTransit.ConsumeContext<T> context)
	        {
	            long value = Interlocked.Increment(ref _count);
	            if (value == _limit)
	                _completed.TrySetResult(value);
	        }
	    }


	    [Test, Explicit]
	    public async void TeeMessagePipe_performance()
	    {
	        var consumer = new TestConsumer<PingMessage>(1500000);

	        var factory = new InstanceAsyncConsumerFactory<TestConsumer<PingMessage>>(consumer);

	        var adapter = new MethodConsumerMessageAdapter<TestConsumer<PingMessage>, PingMessage>();

	        var pipe = new ConsumerMessagePipe<TestConsumer<PingMessage>, PingMessage>(factory, adapter, Retry.None);

	        IInboundMessagePipe inboundPipe = new InboundMessagePipe();
	        var connectHandle = inboundPipe.Connect(pipe);


	        var message = new PingMessage();
	        var context = new TestConsumeContext<PingMessage>(message);

	        for (int i = 0; i < 10; i++)
	        {
                await inboundPipe.Send(context);
	        }

	        Stopwatch timer = Stopwatch.StartNew();

	        const int loopCount = 1500000;
	        for (int i = 0; i < loopCount; i++)
	        {
                await inboundPipe.Send(context);
	        }

	        await consumer.Task;

	        timer.Stop();

	        Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
	        Trace.WriteLine("Messages Per Second: " + loopCount * 1000 / timer.ElapsedMilliseconds);
	    }


	    class TestReceiveContext :
	        MassTransit.ReceiveContext
	    {
	        public CancellationToken CancellationToken { get; private set; }
	        public Stream Body { get; private set; }
	        public TimeSpan ElapsedTime { get; private set; }
	        public Uri InputAddress { get; private set; }
	        public ContentType ContentType { get; private set; }
	        public Encoding ContentEncoding { get; private set; }
	        public bool Redelivered { get; private set; }
	        public Headers Headers { get; private set; }
	        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
	        {
	        }

	        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
	        {
	        }
	    }


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
	        var pipeline = new InboundMessagePipeline(objectRouter, MockRepository.GenerateMock<IInboundPipelineConfigurator>());

	        var message = new PingMessage();
            IConsumeContext context = new ConsumeContext<PingMessage>(ReceiveContext.Empty(), message);

	        for (int i = 0; i < 100; i++)
	        {
	            pipeline.Dispatch(context);
	        }
	        consumer.Reset();

	        Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1500000; i++)
	        {
	            pipeline.Dispatch(context);
	        }

	        timer.Stop();

            Trace.WriteLine("Received: " + (consumer.Count) + ", expected " + 1500000);
	        Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + 1500000 * 1000 / timer.ElapsedMilliseconds);
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
			Trace.WriteLine("Messages Per Second: " + loopCount*1000/timer.ElapsedMilliseconds);
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
			Trace.WriteLine("Messages Per Second: " + loopCount*1000/timer.ElapsedMilliseconds);
		}

		private static MessageRouter<IConsumeContext<PingMessage>> SetupRouterOnly()
		{
			_consumer = new PerformantConsumer<PingMessage>(1500000);

			var messageSink = new InstanceMessageSink<PingMessage>(MultipleHandlerSelector.ForHandler(
				HandlerSelector.ForHandler<PingMessage>(_consumer.Consume)));

			var router = new MessageRouter<IConsumeContext<PingMessage>>();
			router.Connect(messageSink);
			return router;
		}

		private static MessageRouter<IConsumeContext> SetupTwoRoutersOnly()
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

		private static void SendMessages(IPipelineSink<IConsumeContext<PingMessage>> sink, int primeLoopCount)
		{
			var message = new PingMessage();
			var context = new ConsumeContext<PingMessage>(ReceiveContext.Empty(), message);

			for (int i = 0; i < primeLoopCount; i++)
			{
				foreach (var item in sink.Enumerate(context))
				{
					item(context);
				}
			}
		}

		private static void SendMessages(IPipelineSink<IConsumeContext> sink, int primeLoopCount)
		{
			var message = new PingMessage();
			var context = new ConsumeContext<PingMessage>(ReceiveContext.Empty(), message);

			for (int i = 0; i < primeLoopCount; i++)
			{
				foreach (var item in sink.Enumerate(context))
				{
					item(context);
				}
			}
		}
	}


    class TestConsumeContext<TMessage> :
        MassTransit.ConsumeContext<TMessage>
        where TMessage : class
    {
        public TestConsumeContext(TMessage message)
        {
            Message = message;
        }

        public Guid? MessageId { get; private set; }
        public Guid? RequestId { get; private set; }
        public Guid? CorrelationId { get; private set; }
        public DateTime? ExpirationTime { get; private set; }
        public Uri SourceAddress { get; private set; }
        public Uri DestinationAddress { get; private set; }
        public Uri ResponseAddress { get; private set; }
        public Uri FaultAddress { get; private set; }
        public Headers Headers { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public MassTransit.ReceiveContext ReceiveContext { get; private set; }
        public bool HasMessageType(Type messageType)
        {
            return messageType.IsAssignableFrom(typeof(TMessage));
        }

        public bool TryGetMessage<T>(out MassTransit.ConsumeContext<T> consumeContext) 
            where T : class
        {
            consumeContext = this as MassTransit.ConsumeContext<T>;

            return consumeContext != null;
        }

        public Task<SentContext> RespondAsync<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public void Respond<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public void RetryLater()
        {
            throw new NotImplementedException();
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
        }

        public TMessage Message { get; private set; }
        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
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
				HandlerSelector.ForHandler<ClaimModified>(m => { Interlocked.Increment(ref count);  })));

			var messageSink2 = new InstanceMessageSink<ClaimModified>(MultipleHandlerSelector.ForHandler(
				HandlerSelector.ForHandler<ClaimModified>(m => { count2++; })));

            var router = new MessageRouter<IConsumeContext<ClaimModified>>();
            router.Connect(messageSink);
            router.Connect(messageSink2);


            var translater = new InboundConvertMessageSink<ClaimModified>(router);
            var objectRouter = new MessageRouter<IConsumeContext>();
            objectRouter.Connect(translater);
            var pipeline = new InboundMessagePipeline(objectRouter, MockRepository.GenerateMock<IInboundPipelineConfigurator>());

            var message = new ClaimModified();
			var context = new ConsumeContext<ClaimModified>(ReceiveContext.Empty(), message);

            for (int i = 0; i < 100; i++)
            {
				pipeline.Dispatch(context);
            }

            count = 0;
            count2 = 0;

            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < limit; i++)
            {
				pipeline.Dispatch(context);
            }

            timer.Stop();

            Trace.WriteLine("Received: " + (count+count2) + ", expected " + limit * 2);
            Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
            Trace.WriteLine("Messages Per Second: " + limit*1000/timer.ElapsedMilliseconds);
        }
    }

    public class ClaimModified
    {
        
    }
}