// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.SystemView
{
    using System.Windows;
    using Core;
    using Core.Consumer;
    using Log4NetIntegration.Logging;
    using Logging;
    using StructureMap;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static SubscriptionDataConsumer SubscriptionDataConsumer { get; set; }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (SubscriptionDataConsumer != null)
                SubscriptionDataConsumer.Dispose();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.UseLogger(new Log4NetLogger());

            var configuration = new Configuration();

            ObjectFactory.Configure(config =>
                {
                    config.For<IConfiguration>()
                        .Singleton()
                        .Add(context => configuration);

                    config.AddRegistry(new SystemViewRegistry(configuration));
                });

            SubscriptionDataConsumer = ObjectFactory.GetInstance<SubscriptionDataConsumer>();
        }
    }
}