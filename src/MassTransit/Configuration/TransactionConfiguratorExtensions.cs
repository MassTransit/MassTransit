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
namespace MassTransit
{
    using System;
    using PipeConfigurators;
    using Saga;


    public static class TransactionConfiguratorExtensions
    {
        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure the transaction pipe</param>
        public static void UseTransaction(this IPipeConfigurator<ConsumeContext> configurator, Action<ITransactionConfigurator> configure = null)
        {
            ConfigureTransaction(configurator, configure);
        }

        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure the transaction pipe</param>
        public static void UseTransaction<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<ITransactionConfigurator> configure = null)
            where T : class
        {
            ConfigureTransaction(configurator, configure);
        }
        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure the transaction pipe</param>
        public static void UseTransaction<T>(this IPipeConfigurator<SagaConsumeContext<T>> configurator, Action<ITransactionConfigurator> configure = null)
            where T : class, ISaga
        {
            ConfigureTransaction(configurator, configure);
        }
        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure the transaction pipe</param>
        public static void UseTransaction<T>(this IPipeConfigurator<ConsumerConsumeContext<T>> configurator, Action<ITransactionConfigurator> configure = null)
            where T : class
        {
            ConfigureTransaction(configurator, configure);
        }

        static void ConfigureTransaction<T>(IPipeConfigurator<T> configurator, Action<ITransactionConfigurator> configure) 
            where T : class, PipeContext
        {
            var transactionConfigurator = new TransactionPipeSpecification<T>();

            configure?.Invoke(transactionConfigurator);

            configurator.AddPipeSpecification(transactionConfigurator);
        }
    }
}