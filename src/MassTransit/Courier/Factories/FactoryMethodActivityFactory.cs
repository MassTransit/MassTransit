// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.Factories
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public class FactoryMethodActivityFactory<TActivity, TArguments, TLog> :
        ActivityFactory<TArguments, TLog>
        where TActivity : class, ExecuteActivity<TArguments>, CompensateActivity<TLog>
        where TArguments : class
        where TLog : class
    {
        readonly CompensateActivityFactory<TLog> _compensateFactory;
        readonly ExecuteActivityFactory<TArguments> _executeFactory;

        public FactoryMethodActivityFactory(Func<TArguments, TActivity> executeFactory,
            Func<TLog, TActivity> compensateFactory)
        {
            _executeFactory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(executeFactory);
            _compensateFactory = new FactoryMethodCompensateActivityFactory<TActivity, TLog>(compensateFactory);
        }

        public Task Execute(ExecuteContext<TArguments> context, IPipe<ExecuteActivityContext<TArguments>> next)
        {
            return _executeFactory.Execute(context, next);
        }

        public Task Compensate(CompensateContext<TLog> context, IPipe<CompensateActivityContext<TLog>> next)
        {
            return _compensateFactory.Compensate(context, next);
        }
    }
}