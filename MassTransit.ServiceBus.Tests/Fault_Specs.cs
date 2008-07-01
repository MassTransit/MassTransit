namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using MassTransit.ServiceBus.Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using Transports;

	[TestFixture]
	public class When_a_message_fault_occurs :
		Specification
	{
		[Test]
		public void I_should_receive_a_fault_message()
		{
			SmartConsumer sc = new SmartConsumer();

			_bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

			_bus.Subscribe(sc);

			_bus.Publish(new Hello());

			Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(5), true));
		}

		private LocalSubscriptionCache _cache = new LocalSubscriptionCache();
		private IEndpoint _endpoint = new LoopbackEndpoint(new Uri("loopback://localhost/servicebus"));
		private IEndpointResolver _resolver = new EndpointResolver();
		private IServiceBus _bus;
		private IObjectBuilder _builder;

		static When_a_message_fault_occurs()
		{
			EndpointResolver.AddTransport(typeof (LoopbackEndpoint));
		}

		protected override void Before_each()
		{
			_cache = new LocalSubscriptionCache();
			_resolver = new EndpointResolver();
			_endpoint = _resolver.Resolve(new Uri("loopback://localhost/servicebus"));
			_builder = DynamicMock<IObjectBuilder>();
			_bus = new ServiceBus(_endpoint, _builder, _cache, _resolver);
		}

		protected override void After_each()
		{
			_bus.Dispose();
			_endpoint.Dispose();
			_cache.Dispose();
		}

		public class SmartConsumer :
			Consumes<Fault<Hello>>.All
		{
			private readonly ManualResetEvent _gotFault = new ManualResetEvent(false);

			public ManualResetEvent GotFault
			{
				get { return _gotFault; }
			}

			public void Consume(Fault<Hello> message)
			{
				_gotFault.Set();
			}
		}

		[Serializable]
		public class Hello
		{
		}

		[Serializable]
		public class Hi
		{
		}
	}

	[TestFixture]
	public class When_a_correlated_message_fault_is_received :
		Specification
	{
		[Test]
		public void I_should_receive_a_fault_message()
		{
			SmartConsumer sc = new SmartConsumer();

			_bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

			_bus.Subscribe(sc);

			_bus.Publish(new Hello(sc.CorrelationId));

			Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(5), true));
		}

		private LocalSubscriptionCache _cache = new LocalSubscriptionCache();
		private IEndpoint _endpoint = new LoopbackEndpoint(new Uri("loopback://localhost/servicebus"));
		private IEndpointResolver _resolver = new EndpointResolver();
		private IServiceBus _bus;
		private IObjectBuilder _builder;

		static When_a_correlated_message_fault_is_received()
		{
			EndpointResolver.AddTransport(typeof(LoopbackEndpoint));
		}

		protected override void Before_each()
		{
			_cache = new LocalSubscriptionCache();
			_resolver = new EndpointResolver();
			_endpoint = _resolver.Resolve(new Uri("loopback://localhost/servicebus"));
			_builder = DynamicMock<IObjectBuilder>();
			_bus = new ServiceBus(_endpoint, _builder, _cache, _resolver);
		}

		protected override void After_each()
		{
			_bus.Dispose();
			_endpoint.Dispose();
			_cache.Dispose();
		}

		public class SmartConsumer :
			Consumes<Fault<Hello, Guid>>.For<Guid>
		{
			private readonly ManualResetEvent _gotFault = new ManualResetEvent(false);
			private readonly Guid _id = Guid.NewGuid();

			public ManualResetEvent GotFault
			{
				get { return _gotFault; }
			}

			public void Consume(Fault<Hello, Guid> message)
			{
				_gotFault.Set();
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}
		}

		[Serializable]
		public class Hello : CorrelatedBy<Guid>
		{
			private readonly Guid _id;

			public Hello(Guid id)
			{
				_id = id;
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}
		}

		[Serializable]
		public class Hi
		{
		}
	}
}