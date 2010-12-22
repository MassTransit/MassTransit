// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Services.Subscriptions.Server;

    /// <summary>
    ///   Installs the in-memory subscription repository.
    /// </summary>
    public class MassTransitInMemorySubscriptionsInstaller :
        IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Register(
                Component.For<ISubscriptionRepository>()
                    .ImplementedBy<InMemorySubscriptionRepository>()
                    .LifeStyle.Singleton
                );
        }

        // <summary>
        // Registers the in-memory subscription service so that all buses created in the same
        // process share subscriptions
        // </summary>
        //protected void RegisterInMemorySubscriptionService()
        //{
        //    Kernel.Register(
        //        Component.For<IEndpointSubscriptionEvent>().ImplementedBy<LocalSubscriptionService>().LifeStyle.Singleton,
        //        );
        //}
    }
}