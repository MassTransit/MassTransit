// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
using System.Management;

namespace MassTransit.Transports.Msmq.Management
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Microsoft.Win32;

	public class MsmqManagement
	{
		readonly IEnumerable<string> _installedComponents = GetInstalledComponents();
		readonly IEnumerable<string> _requiredComponents;
		readonly IEnumerable<string> _unnecessaryComponents;

		public MsmqManagement()
		{
			_requiredComponents = new[]
				{
					"msmq_Core",
					"msmq_LocalStorage",
					"msmq_MulticastInstalled",
				};

			_unnecessaryComponents = new[]
				{
					"msmq_ADIntegrated",
					"msmq_TriggersService",
					"msmq_TriggersInstalled",
					"msmq_RoutingSupport",
					"msmq_RoutingInstalled",
					"msmq_MQDSService",
					"msmq_MQDSServiceInstalled",
					"msmq_HTTPSupport",
					"msmq_DCOMProxy",
				};
		}

		public bool IsInstalled()
		{
			if (GetMissingComponents().Any())
				return false;

			return true;
		}

		public void Install()
		{
			MsmqInstaller installer;
			switch (GetWindowsVersion())
			{
				case WindowsVersion.TooOldToCare:
				case WindowsVersion.Windows2000:
					throw new NotSupportedException("The Windows version is too old to support automatic installation");

				case WindowsVersion.WindowsXp:
				case WindowsVersion.Windows2003:
					installer = new WindowsServer2003Installer();
					break;

				case WindowsVersion.WindowsVista:
					installer = new WindowsVistaInstaller();
					break;

				case WindowsVersion.Windows2008:
					installer = new WindowsServer2008Installer();
					break;

				case WindowsVersion.Windows2008R2:
					installer = new WindowsServer2008R2Installer();
					break;

				case WindowsVersion.Windows7:
                case WindowsVersion.Windows8:
                case WindowsVersion.Windows81:
                case WindowsVersion.Windows2012:
                case WindowsVersion.Windows2012R2:
                case WindowsVersion.Unknown:         //This is probably a newer version of Windows and the last method might work
					installer = new Windows7Installer();
					break;

				default:
					throw new NotSupportedException("The Windows version was not recognized, installation cannot continue.");
			}

			using (Process process = installer.Install())
			{
				process.WaitForExit();
			}
		}

		IEnumerable<string> GetMissingComponents()
		{
			return _requiredComponents.Except(_installedComponents);
		}

		IEnumerable<string> GetUnnecessaryComponents()
		{
			return _unnecessaryComponents.Except(_installedComponents);
		}

		static WindowsVersion GetWindowsVersion()
		{
			OperatingSystem osInfo = Environment.OSVersion;
			Version version = osInfo.Version;

			// Windows 7 cannot be distinguished from Server 2008 R2 by looking at the
			// version number, so we resort to using the friendly name of the OS
			string osName = (new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get()
			                	.OfType<ManagementObject>()
			                	.Select(x => x.GetPropertyValue("Caption") as string))
			                	.FirstOrDefault() ?? "";
		    var isServer = osName.Contains("Server");

			if (osInfo.Platform == PlatformID.Win32Windows)
				return WindowsVersion.TooOldToCare;

			if (osInfo.Platform != PlatformID.Win32NT)
				return WindowsVersion.TooOldToCare;

			if (version.Major < 5)
				return WindowsVersion.TooOldToCare;

			switch (version.Major)
			{
				case 5:
			        switch (version.Minor)
			        {
                        case 0:
			                return WindowsVersion.Windows2000;
                        case 1:
			                return WindowsVersion.WindowsXp;
                        case 2:
			                return WindowsVersion.Windows2003;
			        }
                    break;
				case 6:
			        switch (version.Minor)
			        {
                        case 0:
                            return isServer ? WindowsVersion.Windows2008 : WindowsVersion.WindowsVista;
                        case 1:
                            return isServer ? WindowsVersion.Windows2008R2 : WindowsVersion.Windows7;
                        case 2:
                            return isServer ? WindowsVersion.Windows2012 : WindowsVersion.Windows8;
                        case 3:
                            return isServer ? WindowsVersion.Windows2012R2 : WindowsVersion.Windows81;
                    }
			        break;
			}

			return WindowsVersion.Unknown;
		}

		static IEnumerable<string> GetInstalledComponents()
		{
			IEnumerable<string> installedComponents = Enumerable.Empty<string>();

			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSMQ\Setup");
			if (registryKey == null)
				return installedComponents;

			using (registryKey)
			{
				installedComponents = registryKey.GetValueNames();
				registryKey.Close();
			}

			return installedComponents;
		}

		public void Start()
		{
			using(var service = new WindowsService("MSMQ"))
			{
				service.Start();
			}
		}
	}
}