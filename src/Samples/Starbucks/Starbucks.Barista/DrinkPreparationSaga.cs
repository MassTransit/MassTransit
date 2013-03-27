// Copyright 2007-2013 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Starbucks.Barista
{
	using System;
	using System.Threading;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Saga;
	using Messages;

	public class DrinkPreparationSaga :
		SagaStateMachine<DrinkPreparationSaga>,
		ISaga
	{
		static DrinkPreparationSaga()
		{
			Define(() =>
				{
					Initially(
						When(NewOrder)
							.Then((saga, message) => saga.ProcessNewOrder(message))
							.TransitionTo(WaitingForPayment)
						);

					During(WaitingForPayment,
					       When(PaymentComplete)
					       	.Then((saga,message) =>
					       		{
					       			Console.WriteLine("Payment Complete for '{0}' got it!", saga.Name);
					       			saga.ServeDrink();
					       		})
					       	.Complete()
						);
				});
		}

		public DrinkPreparationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State PreparingDrink { get; set; }
		public static State WaitingForPayment { get; set; }

		public static Event<NewOrderMessage> NewOrder { get; set; }
		public static Event<PaymentCompleteMessage> PaymentComplete { get; set; }

		public Guid CorrelationId { get; private set; }

		public IServiceBus Bus { get; set; }

		public string Drink { get; set; }
		public string Name { get; set; }

		public void ProcessNewOrder(NewOrderMessage message)
		{
			Name = message.Name;
			Drink = string.Format("{0} {1}", message.Size, message.Item);

			Console.WriteLine("{0} for {1}, got it!", Drink, Name);

			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(i*200);
				Console.WriteLine("[wwhhrrrr....psssss...chrhrhrhrrr]");
			}
		}

		private void ServeDrink()
		{
			Console.WriteLine("I've got a {0} ready for {1}!", Drink, Name);

			var message = new DrinkReadyMessage
				{
					CorrelationId = CorrelationId,
					Drink = Drink,
					Name = Name,
				};

			Bus.Publish(message);
		}
	}
}