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
	using System;
	using System.ServiceProcess;
	using LifeCycles;

	public class InteractiveConfiguration :
		BaseWinServiceConfiguration
	{
		public override IApplicationLifeCycle LifeCycle
		{
			get { throw new NotImplementedException(); }
		}

		public override Credentials Credentials
		{
			get { throw new NotImplementedException(); }
		}

		public override string ServiceName
		{
			get { throw new NotImplementedException(); }
		}

		public override string DisplayName
		{
			get { throw new NotImplementedException(); }
		}

		public override string Description
		{
			get { throw new NotImplementedException(); }
		}

		public override string[] Dependencies
		{
			get { throw new NotImplementedException(); }
		}


		public override void ConfigureServiceProcessInstaller(ServiceProcessInstaller installer)
		{
			installer.Username = null;
			installer.Password = null;
			installer.Account = ServiceAccount.User;
		}
	}
}