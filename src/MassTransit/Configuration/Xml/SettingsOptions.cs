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
namespace MassTransit.Configuration.Xml
{
	using System;
	using System.Collections.Generic;
	using BusConfigurators;

	public class SettingsOptions
    {
        public SettingsOptions()
        {
            Transports = new List<string>();
        }

        public string ReceiveFrom { get; set; }
        public List<string> Transports { get; private set; }
        public string Subscriptions { get; set; }
        public string HealthServiceInterval { get; set; }
		public Action<ServiceBusConfigurator> Callback { get; set; }
	}
}