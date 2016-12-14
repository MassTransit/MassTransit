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
namespace GreenPipes
{
    using System;
    using System.Text;
    using MassTransit.Courier;
    using Partitioning;
    using Specifications;


    public static class ExecuteActivityPartitionerExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            PartitionKeyProvider<ExecuteActivityContext<TActivity, TArguments>> provider = context => keyProvider(context).ToByteArray();

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(provider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, Guid> keyProvider)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            PartitionKeyProvider<ExecuteActivityContext<TActivity, TArguments>> provider = context => keyProvider(context).ToByteArray();

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(provider, partitioner);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitionCount">The number of partitions to use when distributing message delivery</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            int partitionCount, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            PartitionKeyProvider<ExecuteActivityContext<TActivity, TArguments>> provider = context =>
            {
                var key = keyProvider(context) ?? "";
                return textEncoding.GetBytes(key);
            };

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(provider, partitionCount);

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="TActivity"></typeparam>
        /// <typeparam name="TArguments"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="partitioner">An existing partitioner to share</param>
        /// <param name="keyProvider">Provides the key from the message</param>
        /// <param name="encoding">The text encoding to use to convert the string to byte[] (defaults to UTF8)</param>
        public static void UsePartitioner<TActivity, TArguments>(this IPipeConfigurator<ExecuteActivityContext<TActivity, TArguments>> configurator,
            IPartitioner partitioner, Func<ExecuteActivityContext<TActivity, TArguments>, string> keyProvider, Encoding encoding = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (partitioner == null)
                throw new ArgumentNullException(nameof(partitioner));
            if (keyProvider == null)
                throw new ArgumentNullException(nameof(keyProvider));

            var textEncoding = encoding ?? Encoding.UTF8;

            PartitionKeyProvider<ExecuteActivityContext<TActivity, TArguments>> provider = context =>
            {
                var key = keyProvider(context) ?? "";
                return textEncoding.GetBytes(key);
            };

            var specification = new PartitionerPipeSpecification<ExecuteActivityContext<TActivity, TArguments>>(provider, partitioner);

            configurator.AddPipeSpecification(specification);
        }
    }
}