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
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;


    /// <summary>
    /// Configures SSL/TLS for RabbitMQ. See http://www.rabbitmq.com/ssl.html
    /// for details on how to set up RabbitMQ for SSL.
    /// </summary>
    public interface IRabbitMqSslConfigurator
    {
        /// <summary>
        ///The SSL protocol version.
        /// </summary>
        SslProtocols Protocol { get; set; }

        /// <summary>
        /// The server canonical name. The name has to match CN in the server certificate.
        /// </summary>
        string ServerName { get; set; }

        /// <summary>
        /// The path to a file containing a certificate to use for client authentication, not required if <see cref="Certificate"/> is populated
        /// </summary>
        string CertificatePath { get; set; }

        /// <summary>
        /// The password for the certificate file at <see cref="CertificatePath"/>
        /// </summary>
        string CertificatePassphrase { get; set; }

        /// <summary>
        /// A certficate instance to use for client authentication, if provided then <see cref="CertificatePath"/> and <see cref="CertificatePassphrase"/> are not required
        /// </summary>
        X509Certificate Certificate { get; set; }

        /// <summary>
        /// Disables SSL policy checks.
        /// </summary>
        /// <param name="policyErrors">The checks to disable.</param>
        void AllowPolicyErrors(SslPolicyErrors policyErrors);

        /// <summary>
        /// Enables SSL policy checks.
        /// </summary>
        /// <param name="policyErrors">The checks to enable.</param>
        void EnforcePolicyErrors(SslPolicyErrors policyErrors);

        /// <summary>
        /// Whether to use client certificate to authenticate the client. If false, client certificate is used only for SSL encryption.
        /// </summary>
        bool UseCertificateAsAuthenticationIdentity { get; set; }

        /// <summary>
        /// An optional client specified SSL certificate selection callback.  If this is not specified,
        /// the first valid certificate found will be used.
        /// </summary>
        LocalCertificateSelectionCallback CertificateSelectionCallback { get; set; }

        /// <summary>
        /// An optional client specified SSL certificate validation callback.  If this is not specified,
        /// the default callback will be used in conjunction with the <see cref="P:RabbitMQ.Client.SslOption.AcceptablePolicyErrors" /> property to
        /// determine if the remote server certificate is valid.
        /// </summary>
        RemoteCertificateValidationCallback CertificateValidationCallback { get; set; }
    }
}