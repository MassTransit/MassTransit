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
namespace MassTransit.Services.Timeout.Server
{
	using System;
	using Magnum.StateMachine;
	using Messages;
	using Saga;

	public class TimeoutSaga :
		SagaStateMachine<TimeoutSaga>,
		ISaga
	{
		Guid _correlationId;

		static TimeoutSaga()
		{
			Define(() =>
				{
					Correlate(SchedulingTimeout).By(
						(saga, message) => saga.CorrelationId == message.CorrelationId && saga.Tag == message.Tag);
					Correlate(CancellingTimeout).By(
						(saga, message) => saga.CorrelationId == message.CorrelationId && saga.Tag == message.Tag);
					Correlate(CompletingTimeout).By(
						(saga, message) => saga.CorrelationId == message.CorrelationId && saga.Tag == message.Tag);

					Initially(
						When(SchedulingTimeout)
							.Then((saga, message) =>
								{
									saga.Tag = message.Tag;
									saga.TimeoutAt = message.TimeoutAt;

									saga.NotifyTimeoutScheduled();
								}).TransitionTo(WaitingForTime));

					During(WaitingForTime,
						When(CancellingTimeout)
							.Then((saga, message) => { saga.NotifyTimeoutCancelled(); })
							.Complete(),
						When(SchedulingTimeout)
							.Then((saga, message) =>
								{
									saga.TimeoutAt = message.TimeoutAt;

									saga.NotifyTimeoutRescheduled();
								}),
						When(CompletingTimeout)
							.Complete()
						);

					During(Completed,
						When(SchedulingTimeout)
							.Then((saga, message) =>
								{
									saga.TimeoutAt = message.TimeoutAt;

									saga.NotifyTimeoutScheduled();
								}).TransitionTo(WaitingForTime));

					RemoveWhen(x => x.CurrentState == Completed);
				});
		}

		public TimeoutSaga(Guid correlationId)
		{
			_correlationId = correlationId;
		}

		protected TimeoutSaga()
		{
		}


		public static State Initial { get; set; }
		public static State WaitingForTime { get; set; }
		public static State Completed { get; set; }


		public static Event<ScheduleTimeout> SchedulingTimeout { get; set; }
		public static Event<CancelTimeout> CancellingTimeout { get; set; }
		public static Event<TimeoutExpired> CompletingTimeout { get; set; }

		public virtual DateTime TimeoutAt { get; set; }
		public virtual int Tag { get; set; }
		public virtual IServiceBus Bus { get; set; }

		public virtual Guid CorrelationId
		{
			get { return _correlationId; }
			set { _correlationId = value; }
		}

		public virtual bool Equals(TimeoutSaga obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.Tag == Tag && obj.CorrelationId.Equals(CorrelationId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (TimeoutSaga)) return false;
			return Equals((TimeoutSaga) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Tag*397) ^ CorrelationId.GetHashCode();
			}
		}

		void NotifyTimeoutScheduled()
		{
			Bus.Publish(new TimeoutScheduled
				{
					CorrelationId = CorrelationId,
					TimeoutAt = TimeoutAt,
					Tag = Tag,
				});
		}

		void NotifyTimeoutRescheduled()
		{
			Bus.Publish(new TimeoutRescheduled
				{
					CorrelationId = CorrelationId,
					TimeoutAt = TimeoutAt,
					Tag = Tag,
				});
		}

		void NotifyTimeoutCancelled()
		{
			Bus.Publish(new TimeoutCancelled
				{
					CorrelationId = CorrelationId,
					Tag = Tag,
				});
		}
	}
}