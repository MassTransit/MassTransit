// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Amazon;
    using Transport;


    /// <summary>
    /// Settings to configure a AmazonSQS host explicitly without requiring the fluent interface
    /// </summary>
    public interface AmazonSqsHostSettings
    {
        /// <summary>
        ///     The AmazonSQS region to connect
        /// </summary>
        RegionEndpoint Region { get; }

        /// <summary>
        ///     The AccessKey for connecting to the host
        /// </summary>
        string AccessKey { get; }

        /// <summary>
        ///     The password for connection to the host
        ///     MAYBE this should be a SecureString instead of a regular string
        /// </summary>
        string SecretKey { get; }

        /// <summary>
        /// If provided, only <see cref="SendContext"/> headers passing the filter will be copied to the Amazon message attributes
        /// </summary>
        Func<string, bool> CopyHeaderToMessageAttributesFilter { get; }

        Uri HostAddress { get; }

        IConnection CreateConnection();
    }
}
