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

			Learner<string> learner = new Learner<string>(serviceId)
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

			learner.RaiseEvent(Learner<string>.ValueAccepted, message);

			learner.CurrentState.ShouldEqual(Learner<string>.Active);
			learner.Value.ShouldEqual(message.Value);
			learner.BallotId.ShouldEqual(message.BallotId);
		}
	}
}