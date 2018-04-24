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
namespace MassTransit.ActiveMqTransport.Testing
{
    using System;
    using Apache.NMS;
    using Apache.NMS.Util;
    using MassTransit.Testing;


    public class ActiveMqTestHarness :
        BusTestHarness
    {
        Uri _hostAddress;
        Uri _inputQueueAddress;

        public ActiveMqTestHarness(string inputQueueName = null)
        {
            Username = "admin";
            Password = "admin";

            InputQueueName = inputQueueName ?? "input_queue";

            HostAddress = new Uri("activemq://localhost/");
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
        public IActiveMqHost Host { get; private set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IActiveMqBusFactoryConfigurator> OnConfigureActiveMqBus;
        public event Action<IActiveMqBusFactoryConfigurator, IActiveMqHost> OnConfigureActiveMqBusHost;
        public event Action<IActiveMqReceiveEndpointConfigurator> OnConfigureActiveMqReceiveEndoint;
        public event Action<IActiveMqHostConfigurator> OnConfigureActiveMqHost;
        public event Action<ISession> OnCleanupVirtualHost;

        protected virtual void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            OnConfigureActiveMqBus?.Invoke(configurator);
        }

        protected virtual void ConfigureActiveMqBusHost(IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host)
        {
            OnConfigureActiveMqBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            OnConfigureActiveMqReceiveEndoint?.Invoke(configurator);
        }

        protected virtual void ConfigureActiveMqHost(IActiveMqHostConfigurator configurator)
        {
            OnConfigureActiveMqHost?.Invoke(configurator);
        }

        protected virtual void CleanupVirtualHost(ISession session)
        {
            OnCleanupVirtualHost?.Invoke(session);
        }

        protected virtual IActiveMqHost ConfigureHost(IActiveMqBusFactoryConfigurator configurator)
        {
            return configurator.Host(HostAddress, h =>
            {
                h.Username(Username);
                h.Password(Password);

                ConfigureActiveMqHost(h);
            });
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingActiveMq(x =>
            {
                Host = ConfigureHost(x);

                CleanUpVirtualHost(Host);

                ConfigureBus(x);

                ConfigureActiveMqBus(x);

                ConfigureActiveMqBusHost(x, Host);

                x.ReceiveEndpoint(Host, InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureActiveMqReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }

        void CleanUpVirtualHost(IActiveMqHost host)
        {
            try
            {
                using (var connection = host.Settings.CreateConnection())
                using (var model = connection.CreateSession())
                {
                    CleanUpQueue(model, "input_queue");

                    CleanUpQueue(model, InputQueueName);

                    CleanupVirtualHost(model);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void CleanUpQueue(ISession model, string queueName)
        {
            model.DeleteDestination(SessionUtil.GetQueue(model, queueName));
            model.DeleteDestination(SessionUtil.GetQueue(model, $"{queueName}_skipped"));
            model.DeleteDestination(SessionUtil.GetQueue(model, $"{queueName}_error"));
        }
    }
}