namespace MassTransit.Tests.Services.Timeout
{
	using System.Diagnostics;
	using Magnum.Extensions;
    using MassTransit.Services.Timeout.Messages;
    using MassTransit.Services.Timeout.Server;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Fixtures;

    public class TimeoutService_Specs : 
		SagaTestFixture<TimeoutSaga>
    {
        [Test]
        public void messages_should_be_correlated_by_tag_and_id()
        {
            LocalBus.SubscribeSaga(Repository);

            LocalBus.Publish(new ScheduleTimeout(SagaId, 10.Seconds(), 1));
            Saga.ShouldBeInState(TimeoutSaga.WaitingForTime);

            LocalBus.Publish(new CancelTimeout {CorrelationId = SagaId, Tag = 1});

            // this message should be ignored because of the tag
            LocalBus.Publish(new ScheduleTimeout(SagaId, 10.Seconds(), 2));

            Saga.ShouldBeInState(TimeoutSaga.Completed);

        	var filter = new MassTransit.Saga.SagaFilter<TimeoutSaga>(x => true);

        	Trace.WriteLine("Sagas:");
        	foreach (var saga in Repository.Where(filter))
        	{
        		Trace.WriteLine("Saga: " + saga.CorrelationId + ", Tag: " + saga.Tag + ", State: " + saga.CurrentState.Name);
        	}
        }
    }
}