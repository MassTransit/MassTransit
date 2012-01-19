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
namespace MassTransit.EventStoreIntegration.Tests.Specs
{
	using System;
	using Machine.Specifications;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using MassTransit.Tests;
	using MassTransit.Tests.Messages;
	using TestFramework;

	static class BusActions
	{
		//Action<Func< in_memory_event_store_repository = () => 
	}

	public class cashier_got_tired_of_waiting_spec
		: loopback_spec<Cashier>
	{
		Establish context = () =>
			{
				the_cashier = the_cashier_ = specification.CreateSubject();
				the_cashier.had_these_deltas(
					the_cashier.changed_state_to("WaitingForPayment"),
					new Cashier.RememberOrder
						{
							Name = "Machiato Latte",
							Item = "No 4",
							Size = "grande",
							Amount = Cashier.GetPriceForSize("grande")
						});
			};

		static ISagaEventSourced the_cashier;
		static Cashier the_cashier_;

		Because the_customer_was_slow = () => 
			the_cashier.got_tired_of_waiting();

		It timed_out = () => 
			the_cashier_.ShouldBeInState(Cashier.TimedOut);
	}

	public class loopback_spec<T>
		: spec_for<T>
		where T : ISagaEventSourced
	{
		protected static IServiceBus LocalBus { get; private set; }

		Establish the_bus = () =>
			{
				LocalBus = ServiceBusFactory.New(x => { x.ReceiveFrom("loopback://localhost/queue"); });
			};

		Cleanup state = () =>
			{
				LocalBus.Dispose();
				LocalBus = null;
			};

		protected static spec_for<T> specification { get { return new loopback_spec<T>(); } }
	}

	public interface spec_for<T>
		where T : ISagaEventSourced
	{
	}

	public static class F
		
	{
		public static void had_these_deltas<T>(this ISagaEventSourced saga,
			params T[] deltas)
			where T : class
		{
			foreach (var delta in deltas)
				saga.DeltaManager.ApplyStateDelta(delta);
		}

		public static object changed_state_to(this ISagaEventSourced saga,
			string stateName)
		{
			return new SagaStateDelta(StateMachineHelper.GetStateMethod(saga)(stateName));
		}

		public static void got_tired_of_waiting(this ISagaEventSourced saga)
		{
			// HMMMMMM...
		}

		public static T CreateSubject<T>(this spec_for<T> spec)
			where T : ISagaEventSourced
		{
			return CreateSubject(spec, CombGuid.Generate());
		}

		public static T CreateSubject<T>(this spec_for<T> spec, Guid correlationId)
			where T : ISagaEventSourced
		{
			return FastActivator<T>.Create(correlationId);
		}
	}
}