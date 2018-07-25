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
namespace MassTransit.AmazonSqsTransport.Configuration.Configurators
{
    using System;
    using Amazon;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public class ConfigurationHostSettings :
        AmazonSqsHostSettings
    {
        readonly Lazy<Uri> _hostAddress;
        
        public ConfigurationHostSettings()
        {
            _hostAddress = new Lazy<Uri>(FormatHostAddress);
        }

        public RegionEndpoint Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        public AmazonSQSConfig AmazonSqsConfig { get; set; }

        public AmazonSimpleNotificationServiceConfig AmazonSnsConfig { get; set; }

        public Uri HostAddress => _hostAddress.Value;

        public IConnection CreateConnection()
        {
            return new Connection(AccessKey, SecretKey, Region, AmazonSqsConfig, AmazonSnsConfig);
        }

        Uri FormatHostAddress()
        {
            var builder = new UriBuilder
            {
                Scheme = "amazonsqs",
                Host = Region.SystemName,
                Path = "/"
            };

            return builder.Uri;
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = "https",
                Host = Region.SystemName
            }.Uri.ToString();
        }
    }
}
