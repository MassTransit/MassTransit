namespace MassTransit.Tests.Grid.Paxos
{
	using System;
	using MassTransit.Grid.Paxos;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class Sending_an_accepted_value_to_a_listener
	{
		[Test]
		public void Should_advance_it_to_a_valid_state_with_that_value()
		{
			Guid serviceId = Guid.NewGuid();

			Listener<string> listener = new Listener<string>(serviceId)
				{
					Bus = MockRepository.GenerateMock<IServiceBus>()
				};


			Accepted<string> message = new Accepted<string>
				{
					BallotId = 1,
					CorrelationId = serviceId,
					IsFinal = false,
					Value = "Chris",
					ValueBallotId = 1,
				};

			listener.RaiseEvent(Listener<string>.ValueAccepted, message);

			listener.CurrentState.ShouldEqual(Listener<string>.Active);
			listener.Value.ShouldEqual(message.Value);
			listener.BallotId.ShouldEqual(message.BallotId);
		}
	}
}