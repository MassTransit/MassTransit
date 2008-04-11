/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Host
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration.Install;
	using System.Reflection;
	using System.ServiceProcess;
	using log4net;
	using MassTransit.Host.Config.Util.Arguments;
	using Microsoft.Win32;

	public class HostServiceInstaller :
		Installer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (HostServiceInstaller));
		private readonly List<Assembly> _assemblies = new List<Assembly>();
		private readonly ServiceInstaller _serviceInstaller = new ServiceInstaller();
		private readonly ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();
		private string _configuratorType;

		public HostServiceInstaller(string serviceName, string description, string displayName)
		{
			_serviceInstaller.ServiceName = serviceName;
			_serviceInstaller.Description = description;
			_serviceInstaller.DisplayName = displayName;

			Initialize();
		}

		[Argument(Key = "name", Description = "The name for the service", Required = false)]
		public string Name
		{
			set { _serviceInstaller.ServiceName = value; }
		}

		[Argument(Key = "description", Description = "The description for the service", Required = false)]
		public string Description
		{
			set { _serviceInstaller.Description = value; }
		}

		[Argument(Key = "displayname", Description = "The name to display for the service", Required = false)]
		public string DisplayName

		{
			set { _serviceInstaller.DisplayName = value; }
		}

		[Argument(Key = "username", Description = "Username for service account", Required = false)]
		public string Username
		{
			set
			{
				_serviceProcessInstaller.Username = value;
				_serviceProcessInstaller.Account = ServiceAccount.User;
			}
		}

		[Argument(Key = "password", Description = "Password for service account", Required = false)]
		public string Password
		{
			set { _serviceProcessInstaller.Password = value; }
		}

		private void Initialize()
		{
			_serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
			_serviceProcessInstaller.Username = null;
			_serviceProcessInstaller.Password = null;

			_serviceInstaller.StartType = ServiceStartMode.Automatic;

			Installers.AddRange(new Installer[] {_serviceProcessInstaller, _serviceInstaller});
		}

		public bool IsInstalled()
		{
			foreach (ServiceController service in ServiceController.GetServices())
			{
				if (service.ServiceName == _serviceInstaller.ServiceName)
					return true;
			}

			return false;
		}

		public override void Install(IDictionary stateSaver)
		{
            if(_log.IsInfoEnabled)
			    _log.InfoFormat("Installing Service {0}", _serviceInstaller.ServiceName);

			base.Install(stateSaver);

            if(_log.IsInfoEnabled)
			    _log.InfoFormat("Opening Registry");

			using (RegistryKey system = Registry.LocalMachine.OpenSubKey("System"))
			using (RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet"))
			using (RegistryKey services = currentControlSet.OpenSubKey("Services"))
			using (RegistryKey service = services.OpenSubKey(_serviceInstaller.ServiceName, true))
			{
				service.SetValue("Description", _serviceInstaller.Description);

				string imagePath = (string) service.GetValue("ImagePath");

				_log.InfoFormat("Service Path {0}", imagePath);

				// TODO tweak the command line to include the service argument and any other useful stuff

				imagePath += " -service";
				service.SetValue("ImagePath", imagePath);

				using (RegistryKey configuration = service.CreateSubKey("Configuration"))
				{
					configuration.SetValue("Type", _configuratorType);

					_assemblies.ForEach(delegate(Assembly assembly) { configuration.SetValue(assembly.GetName().Name, assembly.Location); });
				}

				service.Close();
				services.Close();
				currentControlSet.Close();
				system.Close();
			}
		}

		public void Unregister()
		{
			if (IsInstalled())
			{
				using (TransactedInstaller ti = new TransactedInstaller())
				{
					ti.Installers.Add(this);

					string path = string.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
					string[] commandLine = {path};

					InstallContext context = new InstallContext(null, commandLine);
					ti.Context = context;

					ti.Uninstall(null);
				}
			}
			else
			{
				Console.WriteLine("Service is not installed");
                if(_log.IsInfoEnabled)
                    _log.Info("Service is not installed");
			}
		}

		public void Register(Assembly configuratorAssembly, string configuratorType)
		{
			if (!IsInstalled())
			{
				_assemblies.Add(configuratorAssembly);
				_configuratorType = configuratorType;

				using (TransactedInstaller ti = new TransactedInstaller())
				{
					ti.Installers.Add(this);

					string path = string.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
					string[] commandLine = {path};

					InstallContext context = new InstallContext(null, commandLine);
					ti.Context = context;

					Hashtable savedState = new Hashtable();

					ti.Install(savedState);
				}
			}
			else
			{
				Console.WriteLine("Service is already installed");
                if (_log.IsInfoEnabled)
                    _log.Info("Service is already installed");
			}
		}
	}
}