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
namespace Starbucks.Cashier
{
	using MassTransit;
	using MassTransit.Saga;
	using Ninject.Modules;
	using Ninject;

    public class CashierRegistry :
		NinjectModule
	{

		public override void Load()
		{
		    Bind<ISagaRepository<CashierSaga>>()
                .To<InMemorySagaRepository<CashierSaga>>();

			Bind<CashierSaga>()
				.To<CashierSaga>();
			
            Bind<CashierService>()
				.To<CashierService>()
				.InSingletonScope();

		    Bus.Initialize(sbc =>
		    {
		        sbc.UseMsmq();
                sbc.UseMulticastSubscriptionClient();
                sbc.ReceiveFrom("msmq://localhost/starbucks_cashier");
                sbc.SetConcurrentConsumerLimit(1); //a cashier cannot multi-task

		        sbc.UseControlBus();

                sbc.Subscribe(subs=>
                {
                    subs.Saga(new InMemorySagaRepository<CashierSaga>());
                });
		    });

		    Bind<IServiceBus>().ToConstant(Bus.Instance());
		}
	}
}