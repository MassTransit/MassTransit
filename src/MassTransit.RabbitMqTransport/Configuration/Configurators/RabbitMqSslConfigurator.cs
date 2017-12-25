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
namespace MassTransit.RabbitMqTransport.Configurators
{
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;


    public class RabbitMqSslConfigurator :
        IRabbitMqSslConfigurator
    {
        SslPolicyErrors _acceptablePolicyErrors;

        public RabbitMqSslConfigurator(RabbitMqHostSettings settings)
        {
            CertificatePath = settings.ClientCertificatePath;
            CertificatePassphrase = settings.ClientCertificatePassphrase;
            Certificate = settings.ClientCertificate;
            UseCertificateAsAuthenticationIdentity = settings.UseClientCertificateAsAuthenticationIdentity;
            ServerName = settings.SslServerName;
            Protocol = settings.SslProtocol;
            _acceptablePolicyErrors = settings.AcceptablePolicyErrors;
            CertificateSelectionCallback = settings.CertificateSelectionCallback;
            CertificateValidationCallback = settings.CertificateValidationCallback;
        }

        public SslPolicyErrors AcceptablePolicyErrors => _acceptablePolicyErrors;

        public void AllowPolicyErrors(SslPolicyErrors policyErrors)
        {
            _acceptablePolicyErrors |= policyErrors;
        }

        public void EnforcePolicyErrors(SslPolicyErrors policyErrors)
        {
            _acceptablePolicyErrors &= ~policyErrors;
        }

        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }

        public X509Certificate Certificate { get; set; }

        public string ServerName { get; set; }

        public SslProtocols Protocol { get; set; }

        public bool UseCertificateAsAuthenticationIdentity { get; set; }

        public LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }

        public RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }

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
    }
}