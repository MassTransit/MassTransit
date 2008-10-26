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
namespace MassTransit.Dashboard
{
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.WindsorExtension;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;
    using ServiceBus.Services.MessageDeferral;
    using ServiceBus.Services.Timeout;
    using WindsorIntegration;

    public class WebAppContainer :
        WindsorContainer
    {
        public WebAppContainer() : base(new XmlInterpreter())
        {
            RegisterFacilities();
            RegisterComponents();
        }

        protected void RegisterFacilities()
        {
            AddFacility("rails", new MonoRailFacility());
            AddFacility("startable", new StartableFacility());
            AddFacility("masstransit", new MassTransitFacility());
        }

        protected void RegisterComponents()
        {
            Register(
                AllTypes.FromAssembly(GetType().Assembly).BasedOn<SmartDispatcherController>(),
                Component.For<RemoteMessageDeferralViewer>(),
                Component.For<RemoteTimeoutViewer>()
                );
        }
    }
}