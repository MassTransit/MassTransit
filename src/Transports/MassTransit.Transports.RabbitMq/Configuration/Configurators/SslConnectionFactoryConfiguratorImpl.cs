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
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Builders;
    using Magnum.Extensions;
    using MassTransit.Configurators;
    using RabbitMQ.Client;


    public class SslConnectionFactoryConfiguratorImpl :
        SslConnectionFactoryConfigurator,
        ConnectionFactoryBuilderConfigurator
    {
        SslPolicyErrors _acceptablePolicyErrors;
        string _certificatePath;
        bool _clientCertificateRequired = true; // Set to true to keep existing implementations
        string _passphrase;
        string _serverName;
        bool _useExplicitCertificate = false;
        AuthMechanismFactory[] _authMechanisms;
        X509Certificate[] _certificates;

        public SslConnectionFactoryConfiguratorImpl()
        {
            _acceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors;
        }

        /// <summary>
        /// Configures the rabbit mq client connection for Sll properties.
        /// </summary>
        /// <param name="builder">Builder with appropriate properties set.</param>
        /// <returns>A connection factory builder</returns>
        /// <remarks>
        /// SSL configuration in Rabbit MQ is a complex topic.  In order to ensure that rabbit can work without client presenting a client certificate
        /// and working just like an SSL enabled web-site which does not require certificate you need to have the following settings in your rabbitmq.config
        /// file.
        ///      {ssl_options, [{cacertfile,"/path_to/cacert.pem"},
        ///            {certfile,"/path_to/server/cert.pem"},
        ///            {keyfile,"/path_to/server/key.pem"},
        ///            {verify,verify_none},
        ///            {fail_if_no_peer_cert,false}]}
        /// The last 2 lines are the important ones.
        /// </remarks>
        public ConnectionFactoryBuilder Configure(ConnectionFactoryBuilder builder)
        {
            builder.Add(connectionFactory =>
                {
                    connectionFactory.Ssl.Enabled = true;
                    if (!_clientCertificateRequired)
                    {
                        // These properties need to be set as empty for the Rabbit MQ client. Null's cause an exception in the client library.
                        connectionFactory.Ssl.CertPath = string.Empty;
                        connectionFactory.Ssl.CertPassphrase = string.Empty;
                        connectionFactory.Ssl.ServerName = string.Empty;
                        // Because no client certificate is present we must allow the remote certificate name mismatch for the connection to succeed.
                        _acceptablePolicyErrors = _acceptablePolicyErrors
                                                  | SslPolicyErrors.RemoteCertificateNameMismatch;
                    }
                    else
                    {
                        if (_useExplicitCertificate)
                        {
                            connectionFactory.Ssl.Certs = new X509CertificateCollection(_certificates);
                        }
                        else
                        {
                            connectionFactory.Ssl.CertPath = _certificatePath;
                            connectionFactory.Ssl.CertPassphrase = _passphrase;
                        }
                        connectionFactory.Ssl.ServerName = _serverName;
                    }
                    connectionFactory.Ssl.AcceptablePolicyErrors = _acceptablePolicyErrors;
                    connectionFactory.Ssl.Version = SslProtocols.Tls;

                    if (_authMechanisms != null && _authMechanisms.Length > 0)
                    {
                        connectionFactory.AuthMechanisms = _authMechanisms;
                    }
                    return connectionFactory;
                });

            return builder;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_serverName.IsEmpty())
            {
                yield return
                    this.Failure("ServerName", "ServerName must be set or allow remote certificate name mismatch");
            }
            if (!_useExplicitCertificate)
            {
                if (_certificatePath.IsEmpty())
                    yield return this.Failure("CertificatePath", "CertificatePath must be specified");
                if (_passphrase.IsEmpty())
                    yield return this.Failure("CertificatePassphrase", "CertificatePassphrase must be specified");
            }
            else
            {
                if (_certificates == null || _certificates.Length == 0)
                    yield return this.Failure("Certificate", "Certificates must be loaded");
            }
        }

        public void SetCertificates(params X509Certificate[] certificates)
        {
            _certificates = certificates;
            if (_certificates != null && _certificates.Length > 0)
            {
                _useExplicitCertificate = true;
            }
        }

        public void SetAcceptablePolicyErrors(SslPolicyErrors policyErrors)
        {
            _acceptablePolicyErrors = policyErrors;
        }

        public void SetClientCertificateRequired(bool clientCertificateRequired)
        {
            _clientCertificateRequired = clientCertificateRequired;
        }

        public void SetServerName(string serverName)
        {
            _serverName = serverName;
        }

        public void SetCertificatePath(string certificatePath)
        {
            _certificatePath = certificatePath;
        }

        public void SetCertificatePassphrase(string passphrase)
        {
            _passphrase = passphrase;
        }

        public void SetAuthMechanisms(params AuthMechanismFactory[] authMechanisms)
        {
            _authMechanisms = authMechanisms;
        }
    }
}