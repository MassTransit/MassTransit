using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using Microsoft.Win32;
using nu.Utility;

namespace MassTransit.Host
{
	public class HostServiceInstaller :
		Installer
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (HostServiceInstaller));
		private readonly List<Assembly> _assemblies = new List<Assembly>();
		private readonly ServiceInstaller _serviceInstaller = new ServiceInstaller();
		private readonly ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();

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

		public void AddAssembly(Assembly assembly)
		{
			_assemblies.Add(assembly);
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
			_log.InfoFormat("Installing Service {0}", _serviceInstaller.ServiceName);

			base.Install(stateSaver);

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

				using (RegistryKey assemblies = service.CreateSubKey("Assemblies"))
				{
					_assemblies.ForEach(delegate(Assembly assembly)
					                    	{
					                    		assemblies.SetValue(assembly.GetName().Name, assembly.Location);
					                    	});
				}

				service.Close();
				services.Close();
				currentControlSet.Close();
				system.Close();
			}
		}
	}
}