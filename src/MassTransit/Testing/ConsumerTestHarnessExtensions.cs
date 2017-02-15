// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing
{
    using System;
    using Pipeline.ConsumerFactories;


    public static class ConsumerTestHarnessExtensions
    {
        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, string queueName = null)
            where T : class, IConsumer, new()
        {
            var consumerFactory = new DefaultConstructorConsumerFactory<T>();

            return new ConsumerTestHarness<T>(harness, consumerFactory, queueName);
        }

        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, IConsumerFactory<T> consumerFactory, string queueName = null)
            where T : class, IConsumer, new()
        {
            return new ConsumerTestHarness<T>(harness, consumerFactory, queueName);
        }

        public static ConsumerTestHarness<T> Consumer<T>(this BusTestHarness harness, Func<T> consumerFactoryMethod, string queueName = null)
            where T : class, IConsumer
        {
            return new ConsumerTestHarness<T>(harness, new DelegateConsumerFactory<T>(consumerFactoryMethod), queueName);
        }
    }
}