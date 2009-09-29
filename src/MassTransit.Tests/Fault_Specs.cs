// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests
{
	using System;
	using System.Reflection;
	using System.Threading;
	using Configuration;
	using Magnum.InterfaceExtensions;
	using MassTransit.Serialization;
	using MassTransit.Transports;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_message_fault_occurs :
		Specification
	{
		private IEndpointFactory _endpointFactory;
		private IServiceBus _bus;
		private IObjectBuilder _builder;

		protected override void Before_each()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_endpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(_builder);
					x.SetDefaultSerializer<XmlMessageSerializer>();
					x.RegisterTransport<LoopbackEndpoint>();
				});
			_builder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(_endpointFactory);
			_bus = ServiceBusConfigurator.New(x =>
				{
					x.SetObjectBuilder(_builder);
					x.ReceiveFrom("loopback://localhost/servicebus");
				});
		}

		protected override void After_each()
		{
			_bus.Dispose();
			_endpointFactory.Dispose();
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

		[Test]
		public void Test_this()
		{
			Type faultType = typeof (Fault<>);

			ConstructorInfo[] constructorInfos = faultType.GetConstructors();

			foreach (ConstructorInfo info in constructorInfos)
			{
				ParameterInfo[] parameters = info.GetParameters();
			}

			Type customFaultType = typeof (Fault<PingMessage>);

			constructorInfos = customFaultType.GetConstructors();
			foreach (ConstructorInfo info in constructorInfos)
			{
				ParameterInfo[] parameters = info.GetParameters();
			}
		}


		[Test]
		public void I_should_receive_a_fault_message()
		{
			SmartConsumer sc = new SmartConsumer();

			_bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

			_bus.Subscribe(sc);

			_bus.Publish(new Hello());

			Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(500), true));
		}
	}

	[TestFixture]
	public class When_a_correlated_message_fault_is_received :
		Specification
	{

		private IEndpointFactory _resolver;
		private IEndpoint _endpoint;
		private ServiceBus _bus;
		private IObjectBuilder _builder;

		protected override void Before_each()
		{
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_resolver = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(_builder);
					x.SetDefaultSerializer<XmlMessageSerializer>();
					x.RegisterTransport<LoopbackEndpoint>();
				});
			_endpoint = _resolver.GetEndpoint(new Uri("loopback://localhost/servicebus"));
			_bus = new ServiceBus(_endpoint, _builder, _resolver);
			_bus.Start();
		}


		protected override void After_each()
		{
			_bus.Dispose();
			_endpoint.Dispose();
			_resolver.Dispose();
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
		public class Hello :
			CorrelatedBy<Guid>
		{
			private Guid _id;

			protected Hello()
			{
			}

			public Hello(Guid id)
			{
				_id = id;
			}

			public Guid CorrelationId
			{
				get { return _id; }
				set { _id = value; }
			}
		}

		[Serializable]
		public class Hi
		{
		}

		[Test]
		public void Open_generics_should_match_properly()
		{
			Assert.IsTrue(new Hello(Guid.NewGuid()).Implements(typeof (CorrelatedBy<>)));
		}

		[Test]
		public void I_should_receive_a_fault_message()
		{
			SmartConsumer sc = new SmartConsumer();

			_bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

			_bus.Subscribe(sc);

			_bus.Publish(new Hello(sc.CorrelationId));

			Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(5), true));
		}
	}
}