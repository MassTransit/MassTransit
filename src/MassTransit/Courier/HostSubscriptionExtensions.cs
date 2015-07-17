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
namespace MassTransit.Courier
{
    using System;
    using Factories;
    using PipeConfigurators;


    public static class HostSubscriptionExtensions
    {
        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator)
            where TActivity : class, ExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, DefaultConstructorExecuteActivityFactory<TActivity, TArguments>.ExecuteFactory);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress)
            where TActivity : class, ExecuteActivity<TArguments>, new()
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, compensateAddress,
                DefaultConstructorExecuteActivityFactory<TActivity, TArguments>.ExecuteFactory);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, Func<TActivity> controllerFactory)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, compensateAddress, _ => controllerFactory());
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator,
            Func<TActivity> controllerFactory)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, _ => controllerFactory());
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator,
            Uri compensateAddress, Func<TArguments, TActivity> controllerFactory)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(controllerFactory);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(
            this IReceiveEndpointConfigurator configurator, Func<TArguments, TActivity> controllerFactory)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var factory = new FactoryMethodExecuteActivityFactory<TActivity, TArguments>(controllerFactory);
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            ExecuteActivityFactory<TArguments> factory)
            where TActivity : ExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, ExecuteActivityFactory<TArguments> factory)
            where TActivity : ExecuteActivity<TArguments>
            where TArguments : class
        {
            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator)
            where TActivity : class, CompensateActivity<TLog>, new()
            where TLog : class
        {
            CompensateActivityHost<TActivity, TLog>(configurator, DefaultConstructorCompensateActivityFactory<TActivity, TLog>.CompensateFactory);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Func<TActivity> controllerFactory)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost<TActivity, TLog>(configurator, _ => controllerFactory());
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Func<TLog, TActivity> controllerFactory)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var factory = new FactoryMethodCompensateActivityFactory<TActivity, TLog>(controllerFactory);
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, CompensateActivityFactory<TLog> factory)
            where TActivity : CompensateActivity<TLog>
            where TLog : class
        {
            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configurator.AddEndpointSpecification(specification);
        }
    }
}