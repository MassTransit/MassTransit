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
namespace MassTransit
{
    using System;
    using Context.SetCorrelationIds;
    using Topology;
    using Topology.Conventions;
    using Topology.Conventions.CorrelationId;


    public static class CorrelationIdConventionExtensions
    {
        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<T, Guid> correlationIdSelector)
            where T : class
        {
            configurator.AddOrUpdateConvention<ICorrelationIdMessageSendTopologyConvention<T>>(
                () =>
                {
                    var convention = new CorrelationIdMessageSendTopologyConvention<T>();
                    convention.SetCorrelationId(new DelegateSetCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new DelegateSetCorrelationId<T>(correlationIdSelector));

                    return update;
                });
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this IMessageSendTopologyConfigurator<T> configurator, Func<T, Guid?> correlationIdSelector)
            where T : class
        {
            configurator.AddOrUpdateConvention<ICorrelationIdMessageSendTopologyConvention<T>>(
                () =>
                {
                    var convention = new CorrelationIdMessageSendTopologyConvention<T>();
                    convention.SetCorrelationId(new NullableDelegateSetCorrelationId<T>(correlationIdSelector));

                    return convention;
                },
                update =>
                {
                    update.SetCorrelationId(new NullableDelegateSetCorrelationId<T>(correlationIdSelector));

                    return update;
                });
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this ISendTopology configurator, Func<T, Guid> correlationIdSelector)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseCorrelationId(correlationIdSelector);
        }

        /// <summary>
        /// Specify for the message type that the delegate be used for setting the CorrelationId
        /// property of the message envelope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="correlationIdSelector"></param>
        public static void UseCorrelationId<T>(this ISendTopology configurator, Func<T, Guid?> correlationIdSelector)
            where T : class
        {
            configurator.GetMessageTopology<T>().UseCorrelationId(correlationIdSelector);
        }
    }
}