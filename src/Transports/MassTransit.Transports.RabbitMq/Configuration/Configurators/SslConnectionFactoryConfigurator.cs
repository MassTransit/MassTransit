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
namespace MassTransit.Transports.RabbitMq.Configuration.Configurators
{
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using RabbitMQ.Client;


    /// <summary>
    /// Configures SSL/TLS for RabbitMQ. See http://www.rabbitmq.com/ssl.html
    /// for details on how to set up RabbitMQ for SSL.
    /// </summary>
    public interface SslConnectionFactoryConfigurator
    {
        void SetServerName(string serverName);
        void SetCertificatePath(string certificatePath);
        void SetCertificatePassphrase(string passphrase);
        void SetCertificates(params X509Certificate[] certificates);
        void SetAcceptablePolicyErrors(SslPolicyErrors policyErrors);
        void SetClientCertificateRequired(bool clientCertificateRequired);
        void SetAuthMechanisms(params AuthMechanismFactory[] factories);
    }
}