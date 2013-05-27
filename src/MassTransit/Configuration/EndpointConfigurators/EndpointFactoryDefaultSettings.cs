// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EndpointConfigurators
{
    using System;
    using System.Transactions;
    using Magnum.Extensions;
    using Serialization;
    using Transports;
    using Util;

    public class EndpointFactoryDefaultSettings :
        IEndpointFactoryDefaultSettings
    {
        public EndpointFactoryDefaultSettings()
        {
            CreateMissingQueues = true;
            CreateTransactionalQueues = false;
            PurgeOnStartup = false;
            RequireTransactional = false;
            Serializer = new XmlMessageSerializer();
            var messageSerializers = new SupportedMessageSerializers();
            messageSerializers.AddSerializer(Serializer);
            SupportedSerializers = messageSerializers;
            TransactionTimeout = 30.Seconds();
            IsolationLevel = IsolationLevel.Serializable;
            RetryLimit = 5;
            TrackerFactory = DefaultTrackerFactory;
        }

        static IInboundMessageTracker DefaultTrackerFactory(int retryLimit)
        {
            return new InMemoryInboundMessageTracker(retryLimit);
        }

        public EndpointFactoryDefaultSettings([NotNull] IEndpointFactoryDefaultSettings defaults)
        {
            if (defaults == null)
                throw new ArgumentNullException("defaults");

            CreateMissingQueues = defaults.CreateMissingQueues;
            CreateTransactionalQueues = defaults.CreateTransactionalQueues;
            IsolationLevel = defaults.IsolationLevel;
            PurgeOnStartup = defaults.PurgeOnStartup;
            RequireTransactional = defaults.RequireTransactional;
            Serializer = defaults.Serializer;
            SupportedSerializers = defaults.SupportedSerializers;
            TransactionTimeout = defaults.TransactionTimeout;
            RetryLimit = defaults.RetryLimit;
            TrackerFactory = defaults.TrackerFactory;
        }

        public ISupportedMessageSerializers SupportedSerializers { get; set; }
        public bool CreateMissingQueues { get; set; }
        public bool CreateTransactionalQueues { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public int RetryLimit { get; set; }
        public MessageTrackerFactory TrackerFactory { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public bool PurgeOnStartup { get; set; }
        public bool RequireTransactional { get; set; }
        public TimeSpan TransactionTimeout { get; set; }

        public EndpointSettings CreateEndpointSettings(IEndpointAddress address)
        {
            var settings = new EndpointSettings(address)
                {
                    Serializer = Serializer,
                    SupportedSerializers = SupportedSerializers,
                    CreateIfMissing = CreateMissingQueues,
                    TransactionTimeout = TransactionTimeout,
                    PurgeExistingMessages = PurgeOnStartup,
                    RequireTransactional = RequireTransactional,
                    IsolationLevel = IsolationLevel,
                    Transactional = CreateTransactionalQueues,
                    RetryLimit = RetryLimit,
                    TrackerFactory = TrackerFactory
                };

            return settings;
        }
    }
}