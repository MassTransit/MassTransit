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
namespace HealthServiceHost
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using Microsoft.Practices.ServiceLocation;

    public class HealthServiceConfiguration :
        InteractiveConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public HealthServiceConfiguration(IServiceLocator serviceLocator)
        {
            _lifecycle = new HealthServiceLifeCycle(serviceLocator);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }

        public override string ServiceName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string DisplayName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string Description
        {
            get { return "This service manages the health for Mass Transit"; }
        }

        public override string[] Dependencies
        {
            get { return new[] {"MSMQ"}; }
        }
    }
}