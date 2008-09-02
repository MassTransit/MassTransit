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
namespace MassTransit.Host.Configurations
{
	using System.ServiceProcess;
	using LifeCycles;

	public abstract class BaseWinServiceConfiguration :
		IInstallationConfiguration
	{
		public abstract Credentials Credentials { get; }
		public abstract string ServiceName { get; }
		public abstract string DisplayName { get; }
		public abstract string Description { get; }
		public abstract string[] Dependencies { get; }

		public abstract IApplicationLifeCycle LifeCycle { get; }

		public virtual void ConfigureServiceInstaller(ServiceInstaller installer)
		{
			installer.ServiceName = this.ServiceName;
			installer.Description = this.Description;
			installer.DisplayName = this.DisplayName;
			installer.ServicesDependedOn = this.Dependencies;
			installer.StartType = ServiceStartMode.Automatic;
		}

		public abstract void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer);
	}
}