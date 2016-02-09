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

namespace MassTransit.AzureServiceBusTransport.Hosting
{
    using System;
    using MassTransit.Hosting;


    /// <summary>
    /// Represents the Advanced Message Queuing Protocol transport settings.
    /// </summary>
    /// <remarks>
    /// All properties are nullable so that default values are not overwritten unless
    /// a configuraiton value is provided.
    /// </remarks>
    public interface ServiceBusAmqpTransportSettings :
        ISettings
    {
        /// <summary>
        /// Sets the batch flush interval to use with the messaging factory, default is 20ms.
        /// </summary>
        /// <remarks>
        /// Currently the Microsoft ServiceBus client defaults to 20ms. For more inforamtion
        /// regarding batching and performance see: 
        /// https://azure.microsoft.com/en-us/blog/new-article-best-practices-for-performance-improvements-using-service-bus-brokered-messaging/
        /// </remarks>
        TimeSpan? BatchFlushInterval { get; }
        bool? EnableLinkRedirect { get; }
        int? MaxFrameSize { get; }
        bool? UseSslStreamSecurity { get; }
    }
}