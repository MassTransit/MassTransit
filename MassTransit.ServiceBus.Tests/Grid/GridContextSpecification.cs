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
namespace MassTransit.ServiceBus.Tests.Grid
{
    using System.IO;
    using MassTransit.ServiceBus.Internal;
    using WindsorIntegration;

    public abstract class GridContextSpecification :
        Specification
    {
        protected readonly DefaultMassTransitContainer _container = new DefaultMassTransitContainer("castle.xml");
        protected IServiceBus _bus;
        protected IObjectBuilder _builder;
        protected IEndpointResolver _endpointResolver;

        static GridContextSpecification()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
        }

        protected override void Before_each()
        {
            _bus = _container.Resolve<IServiceBus>();
            _builder = _container.Resolve<IObjectBuilder>();
            _endpointResolver = _container.Resolve<IEndpointResolver>();
        }
    }
}