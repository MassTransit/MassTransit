// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using Courier;
    using Courier.Factories;
    using PipeConfigurators;


    public static class CourierHostConfiguratorExtensions
    {
        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            ExecuteActivityHost(configurator, DefaultConstructorExecuteActivityFactory<TActivity, TArguments>.ExecuteFactory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            ExecuteActivityHost(configurator, compensateAddress, DefaultConstructorExecuteActivityFactory<TActivity, TArguments>.ExecuteFactory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, compensateAddress, _ => activityFactory(), configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Func<TActivity> activityFactory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, _ => activityFactory(), configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, Func<TArguments, TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(activityFactory);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            Func<TArguments, TActivity> activityFactory,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(activityFactory);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            ExecuteActivityFactory<TActivity, TArguments> factory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            ExecuteActivityFactory<TActivity, TArguments> factory, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>, new()
            where TLog : class
        {
            CompensateActivityHost(configurator, DefaultConstructorCompensateActivityFactory<TActivity, TLog>.CompensateFactory, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Func<TActivity> controllerFactory,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost(configurator, _ => controllerFactory(), configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Func<TLog, TActivity> controllerFactory,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var factory = new FactoryMethodCompensateActivityFactory<TActivity, TLog>(controllerFactory);
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator,
            CompensateActivityFactory<TActivity, TLog> factory, Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
