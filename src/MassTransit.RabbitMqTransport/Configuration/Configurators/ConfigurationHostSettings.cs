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
namespace MassTransit.RabbitMqTransport.Configuration.Configurators
{
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using RabbitMQ.Client;
    using Transports;


    class ConfigurationHostSettings :
        RabbitMqHostSettings
    {
        public ConfigurationHostSettings()
        {
            MessageNameFormatter = new RabbitMqMessageNameFormatter();

            var connectionFactory = new ConnectionFactory();
            SslProtocol = connectionFactory.Ssl.Version;
            AcceptablePolicyErrors = connectionFactory.Ssl.AcceptablePolicyErrors;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort Heartbeat { get; set; }
        public bool Ssl { get; set; }
        public SslProtocols SslProtocol { get; set; }
        public string SslServerName { get; set; }
        public SslPolicyErrors AcceptablePolicyErrors { get; set; }
        public string ClientCertificatePath { get; set; }
        public string ClientCertificatePassphrase { get; set; }
        public X509Certificate ClientCertificate { get; set; }
        public bool UseClientCertificateAsAuthenticationIdentity { get; set; }
        public IMessageNameFormatter MessageNameFormatter { get; set; }
    }
}