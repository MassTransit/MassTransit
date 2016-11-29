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
    using Pipeline.ConsumerFactories;


    public static class TestFactorySelectorExtensions
    {
        /// <summary>
        /// Selects the Consumer test factory, which can then be used to select a transport for the test
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IConsumerTestFactorySelector<TConsumer> ForConsumer<TConsumer>(this ITestFactorySelector selector)
            where TConsumer : class, IConsumer, new()
        {
            var consumerFactory = new DefaultConstructorConsumerFactory<TConsumer>();

            return new ConsumerTestFactorySelector<TConsumer>(consumerFactory);
        }
    }
}