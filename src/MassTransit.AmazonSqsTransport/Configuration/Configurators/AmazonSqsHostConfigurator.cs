// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Configuration.Configurators
{
    using System;
    using Amazon;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Exceptions;


    public class AmazonSqsHostConfigurator :
        IAmazonSqsHostConfigurator
    {
        readonly ConfigurationHostSettings _settings;

        public AmazonSqsHostConfigurator(Uri address)
        {
            if (string.Compare("amazonsqs", address.Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                throw new AmazonSqsTransportConfigurationException($"The address scheme was invalid: {address.Scheme}");

            var regionEndpoint = RegionEndpoint.GetBySystemName(address.Host);

            _settings = new ConfigurationHostSettings
            {
                Region = regionEndpoint,
                AccessKey = string.Empty,
                SecretKey = string.Empty,
                SessionToken = string.Empty,
                AmazonSqsConfig = new AmazonSQSConfig { RegionEndpoint = regionEndpoint },
                AmazonSnsConfig = new AmazonSimpleNotificationServiceConfig { RegionEndpoint = regionEndpoint },
            };

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                string[] parts = address.UserInfo.Split(':');
                _settings.AccessKey = parts[0];

                if (parts.Length >= 2)
                    _settings.SecretKey = parts[1];
            }
        }

        public AmazonSqsHostSettings Settings => _settings;

        public void AccessKey(string accessKey)
        {
            _settings.AccessKey = accessKey;
        }

        public void SecretKey(string secretKey)
        {
            _settings.SecretKey = secretKey;
        }

        public void SessionToken(string sessionToken)
        {
            _settings.SessionToken = sessionToken;
        }

        public void Config(AmazonSQSConfig config)
        {
            _settings.AmazonSqsConfig = config;
        }

        public void Config(AmazonSimpleNotificationServiceConfig config)
        {
            _settings.AmazonSnsConfig = config;
        }
    }
}