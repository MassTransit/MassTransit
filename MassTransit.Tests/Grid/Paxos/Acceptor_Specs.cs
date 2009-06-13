namespace MassTransit.Tests.Grid.Paxos
{
	using System;
	using MassTransit.Grid.Paxos;
	using MassTransit.Internal;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class An_acceptor_receiving_a_prepare
	{
		private IEndpoint _endpoint;
		private IEndpointFactory _endpointFactory;
		private IObjectBuilder _builder;
		private IServiceBus _bus;
		private Guid _serviceId;
		private Guid _leaderId;

		[SetUp]
		public void Setup()
		{
			_serviceId = Guid.NewGuid();
			_leaderId = Guid.NewGuid();

			_endpoint = MockRepository.GenerateMock<IEndpoint>();
			_endpoint.Expect(x => x.Send<Promise<string>>(null)).IgnoreArguments();

			_endpointFactory = MockRepository.GenerateMock<IEndpointFactory>();
			_endpointFactory.Stub(x => x.GetEndpoint((Uri)null)).IgnoreArguments().Return(_endpoint);

			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_builder.Stub(x => x.GetInstance<IEndpointFactory>()).Return(_endpointFactory);

			_bus = MockRepository.GenerateMock<IServiceBus>();
		}

		[Test]
		public void Should_accept_the_value()
		{
			var acceptor = new Acceptor<string>(_serviceId)
			{
				Bus = _bus,
			};

			InboundMessageHeaders.SetCurrent(x =>
				{
					x.ReceivedOn(_bus);
					x.SetObjectBuilder(_builder);
					x.SetResponseAddress("loopback://localhost/queue");
				});

			Prepare<string> prepare = new Prepare<string>
				{
					BallotId = 1,
					CorrelationId = _serviceId,
					LeaderId = _leaderId,
				};

			acceptor.RaiseEvent(Acceptor<string>.Prepare, prepare);

			acceptor.CurrentState.ShouldEqual(Acceptor<string>.Prepared);

			var accept = new Accept<string>
			{
				BallotId = 1,
				CorrelationId = _serviceId,
				LeaderId = _leaderId,
				Value = "Chris",
			};

			acceptor.RaiseEvent(Acceptor<string>.Accept, accept);

			acceptor.CurrentState.ShouldEqual(Acceptor<string>.SteadyState);
			acceptor.Value.ShouldEqual(accept.Value);
			acceptor.BallotId.ShouldEqual(accept.BallotId);

			_endpoint.VerifyAllExpectations();
		}
	}
}