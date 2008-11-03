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
namespace MassTransit.Host.Actions
{
	using log4net;
	using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Install the host as a windows service
    /// </summary>
	public class InstallServiceAction :
		IAction
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (InstallServiceAction));

		public void Do(IInstallationConfiguration configuration, IServiceLocator serviceLocator)
		{
			_log.Info("Received service install notification");

			if (HostServiceInstaller.IsInstalled(configuration))
			{
				string message = string.Format("The {0} service has already been installed.", configuration.Settings.ServiceName);
				_log.Error(message);

				return;
			}

			new HostServiceInstaller(configuration)
				.Register();
		}
	}
}