using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.ActiveMqTransport
{
    public static class ActiveMqArtemisSupport
    {
        /// <summary>
        /// This will allow to specify the property TemporaryQueueNamePrefix,
        /// otherwise that property will be ignored
        /// </summary>
        public static bool EnableNamespaceSupport { get; set; } = false;
        /// <summary>
        /// will be used a prefix for the queue name when creating temporary queues.
        /// Temporary queues are used by e.g. command reply pattern (IRequestClient&lt;&gt;)
        /// If dot seperated then you need to include the dot at the end of the prefix
        /// </summary>
        public static string TemporaryQueueNamePrefix { get; set; } = string.Empty;
        /// <summary>
        /// Artemis changed the name for virtual topics (FQQN)
        /// https://activemq.apache.org/components/artemis/migration-documentation/VirtualTopics.html
        /// Enabling this property will enable support for the new syntax
        /// </summary>
        public static bool EnableArtemisVirtualTopicNamingSupport { get; set; } = false;
        /// <summary>
        /// Enables some extra logging. Could be interesting to verify if failover is working
        /// as expected.
        /// </summary>
        public static bool EnableExtraConnectionLogging { get; set; } = false;


    }
}
