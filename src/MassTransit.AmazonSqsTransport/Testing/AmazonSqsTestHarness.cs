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
namespace MassTransit.AmazonSqsTransport.Testing
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Configuration;
    using MassTransit.Testing;


    public class AmazonSqsTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public AmazonSqsTestHarness(string inputQueueName = null)
        {
            Username = "admin";
            Password = "admin";

            InputQueueName = inputQueueName ?? "input_queue";

            HostAddress = new Uri("amazonsqs://localhost/");
        }

        public Uri HostAddress
        {
            get => _hostAddress;
            set
            {
                _hostAddress = value;
                _inputQueueAddress = new Uri(HostAddress, InputQueueName);
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string InputQueueName { get; }
        public IAmazonSqsHost Host { get; private set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IAmazonSqsBusFactoryConfigurator> OnConfigureAmazonSqsBus;
        public event Action<IAmazonSqsBusFactoryConfigurator, IAmazonSqsHost> OnConfigureAmazonSqsBusHost;
        public event Action<IAmazonSqsReceiveEndpointConfigurator> OnConfigureAmazonSqsReceiveEndoint;
        public event Action<IAmazonSqsHostConfigurator> OnConfigureAmazonSqsHost;
        public event Action<IAmazonSQS, IAmazonSimpleNotificationService> OnCleanupVirtualHost;

        protected virtual void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            OnConfigureAmazonSqsBus?.Invoke(configurator);
        }

        protected virtual void ConfigureAmazonSqsBusHost(IAmazonSqsBusFactoryConfigurator configurator, IAmazonSqsHost host)
        {
            OnConfigureAmazonSqsBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            OnConfigureAmazonSqsReceiveEndoint?.Invoke(configurator);
        }

        protected virtual void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
            OnConfigureAmazonSqsHost?.Invoke(configurator);
        }

        protected virtual void CleanupVirtualHost(IAmazonSQS amazonSqs, IAmazonSimpleNotificationService amazonSns)
        {
            OnCleanupVirtualHost?.Invoke(amazonSqs, amazonSns);
        }

        protected virtual IAmazonSqsHost ConfigureHost(IAmazonSqsBusFactoryConfigurator configurator)
        {
            return configurator.Host(HostAddress, h =>
            {
                h.AccessKey(Username);
                h.SecretKey(Password);

                ConfigureAmazonSqsHost(h);
            });
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAmazonSqs(x =>
            {
                Host = ConfigureHost(x);

                CleanUpVirtualHost(Host);

                ConfigureBus(x);

                ConfigureAmazonSqsBus(x);

                ConfigureAmazonSqsBusHost(x, Host);

                x.ReceiveEndpoint(Host, InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureAmazonSqsReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }

        void CleanUpVirtualHost(IAmazonSqsHost host)
        {
            try
            {
                using (var connection = host.Settings.CreateConnection())
                {
                    var amazonSqs = connection.CreateAmazonSqs();
                    var amazonSns = connection.CreateAmazonSns();

                    CleanUpQueue(amazonSqs, amazonSns, "input_queue");

                    CleanUpQueue(amazonSqs, amazonSns, InputQueueName);

                    CleanupVirtualHost(amazonSqs, amazonSns);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void CleanUpQueue(IAmazonSQS amazonSqs, IAmazonSimpleNotificationService amazonSns, string queueName)
        {
            async Task CleanUpQueue(string queue)
            {
                var topicArn = (await amazonSns.CreateTopicAsync(queueName)).TopicArn;
                await amazonSns.DeleteTopicAsync(topicArn);

                var queueUrl = (await amazonSqs.GetQueueUrlAsync(queue)).QueueUrl;
                await amazonSqs.DeleteQueueAsync(queueUrl);
            }

            Task.Run(async () =>
            {
                await CleanUpQueue(queueName).ConfigureAwait(false);
                await CleanUpQueue($"{queueName}_skipped").ConfigureAwait(false);
                await CleanUpQueue($"{queueName}_error").ConfigureAwait(false);
            });
        }
    }
}