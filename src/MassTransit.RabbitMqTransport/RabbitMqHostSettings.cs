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
namespace MassTransit.RabbitMqTransport
{
    using System.Net.Security;


    /// <summary>
    /// Settings to configure a RabbitMQ host explicitly without requiring the fluent interface
    /// </summary>
    public interface RabbitMqHostSettings
    {
        /// <summary>
        ///     The RabbitMQ host to connect to (should be a valid hostname)
        /// </summary>
        string Host { get; }

        /// <summary>
        ///     The RabbitMQ port to connect
        /// </summary>
        int Port { get; }

        /// <summary>
        ///     The virtual host for the connection
        /// </summary>
        string VirtualHost { get; }

        /// <summary>
        ///     The Username for connecting to the host
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     The password for connection to the host
        ///     MAYBE this should be a SecureString instead of a regular string
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     The heartbeat interval (in seconds) to keep the host connection alive
        /// </summary>
        ushort Heartbeat { get; }

        /// <summary>
        /// True if SSL is required
        /// </summary>
        bool Ssl { get; }

        /// <summary>
        /// The server name specified on the certificate for the RabbitMQ server
        /// </summary>
        string SslServerName { get; }

        /// <summary>
        /// The acceptable policy errors for the SSL connection
        /// </summary>
        SslPolicyErrors AcceptablePolicyErrors { get; }

        /// <summary>
        /// The path to the client certificate if client certificate authentication is used
        /// </summary>
        string ClientCertificatePath { get; }

        /// <summary>
        /// The passphrase for the client certificate 
        /// </summary>
        string ClientCertificatePassphrase { get; }
    }
}