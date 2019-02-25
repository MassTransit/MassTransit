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
namespace MassTransit.AmazonSqsTransport.Configuration
{
    using Amazon.Runtime;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;


    public interface IAmazonSqsHostConfigurator
    {
        /// <summary>
        /// Sets the accessKey for the connection to AmazonSQS/AmazonSNS
        /// </summary>
        /// <param name="accessKey"></param>
        void AccessKey(string accessKey);

        /// <summary>
        /// Sets the secretKey for the connection to AmazonSQS/AmazonSNS
        /// </summary>
        /// <param name="secretKey"></param>
        void SecretKey(string secretKey);

        /// <summary>
        /// Sets the credentials for the connection to AmazonSQS/AmazonSNS
        /// This is an alternative to using AccessKey() and SecretKey()
        /// See https://docs.aws.amazon.com/sdkfornet1/latest/apidocs/html/T_Amazon_Runtime_AWSCredentials.htm for usages
        /// </summary>
        /// <param name="credentials"></param>
        void Credentials(AWSCredentials credentials);

        /// <summary>
        /// Sets the default config for the connection to AmazonSQS
        /// </summary>
        /// <param name="config"></param>
        void Config(AmazonSQSConfig config);

        /// <summary>
        /// Sets the default config for the connection to AmazonSNS
        /// </summary>
        /// <param name="config"></param>
        void Config(AmazonSimpleNotificationServiceConfig config);
    }
}