// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Builders;
	using Magnum.Extensions;
	using MassTransit.Configurators;

	public class SslConnectionFactoryConfiguratorImpl :
		SslConnectionFactoryConfigurator,
		ConnectionFactoryBuilderConfigurator
	{
		SslPolicyErrors _acceptablePolicyErrors;
		string _certificatePath;
		string _passphrase;
		string _serverName;

		public SslConnectionFactoryConfiguratorImpl()
		{
			_acceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors;
		}

		public ConnectionFactoryBuilder Configure(ConnectionFactoryBuilder builder)
		{
			builder.Add(connectionFactory =>
				{
					connectionFactory.Ssl.Enabled = true;
					connectionFactory.Ssl.CertPath = _certificatePath;
					connectionFactory.Ssl.CertPassphrase = _passphrase;
					connectionFactory.Ssl.ServerName = _serverName;
					connectionFactory.Ssl.AcceptablePolicyErrors = _acceptablePolicyErrors;
					connectionFactory.Ssl.Version = SslProtocols.Tls;

					return connectionFactory;
				});

			return builder;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_serverName.IsEmpty())
				yield return this.Failure("ServerName", "ServerName must be set or allow remote certificate name mismatch");
			if (_certificatePath.IsEmpty())
				yield return this.Failure("CertificatePath", "CertificatePath must be specified");
			if (_passphrase.IsEmpty())
				yield return this.Failure("CertificatePassphrase", "CertificatePassphrase must be specified");
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
	}
}