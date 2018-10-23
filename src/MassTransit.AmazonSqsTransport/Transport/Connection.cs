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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using Amazon;
    using Amazon.Lambda;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public class Connection :
        IConnection
    {
        readonly string _accessKey;
        readonly AmazonSimpleNotificationServiceConfig _amazonSnsConfig;
        readonly AmazonSQSConfig _amazonSqsConfig;
        readonly string _secretKey;
        readonly AmazonLambdaConfig _amazonLambdaConfig;

        public Connection(string accessKey, string secretKey,
            RegionEndpoint regionEndpoint = null,
            AmazonSQSConfig amazonSqsConfig = null,
            AmazonSimpleNotificationServiceConfig amazonSnsConfig = null,
            AmazonLambdaConfig amazonLambdaConfig = null)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
            _amazonSqsConfig = amazonSqsConfig ?? new AmazonSQSConfig {RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1};
            _amazonSnsConfig = amazonSnsConfig ?? new AmazonSimpleNotificationServiceConfig {RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1};
            _amazonLambdaConfig = amazonLambdaConfig ?? new AmazonLambdaConfig {RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1};
        }

        public IAmazonSQS CreateAmazonSqsClient()
        {
            return new AmazonSQSClient(_accessKey, _secretKey, _amazonSqsConfig);
        }

        public IAmazonSimpleNotificationService CreateAmazonSnsClient()
        {
            return new AmazonSimpleNotificationServiceClient(_accessKey, _secretKey, _amazonSnsConfig);
        }

        public IAmazonLambda CreateAmazonLambdaClient()
        {
            return new AmazonLambdaClient(_accessKey, _secretKey, _amazonLambdaConfig);
        }
    }
}
