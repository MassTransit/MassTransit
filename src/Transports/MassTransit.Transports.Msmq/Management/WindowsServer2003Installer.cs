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
namespace MassTransit.Transports.Msmq.Management
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Logging;

    public class WindowsServer2003Installer :
		MsmqInstaller
	{
		static readonly ILog _log = Logger.Get(typeof (WindowsServer2003Installer));
		readonly IEnumerable<string> _installComponents;
		readonly IEnumerable<string> _unsupportedComponents;

		public WindowsServer2003Installer()
		{
			_installComponents = new[]
				{
					"msmq_Core",
					"msmq_LocalStorage",
				};

			_unsupportedComponents = new[]
				{
					"msmq_MulticastInstalled",
				};
		}

		public Process Install()
		{
			_log.DebugFormat("Installing MSMQ on Windows Server 2003");

			string fileName = Path.GetTempFileName();

			using (StreamWriter w = File.CreateText(fileName))
			{
				w.WriteLine("[Version]");
				w.WriteLine("Signature = \"$Windows NT$\"");
				w.WriteLine();
				w.WriteLine("[Global]");
				w.WriteLine("FreshMode = Custom");
				w.WriteLine("MaintenanceMode = RemoveAll");
				w.WriteLine("UpgradeMode = UpgradeOnly");
				w.WriteLine();
				w.WriteLine("[Components]");

				foreach (string s in _installComponents.Except(_unsupportedComponents))
					w.WriteLine(s + " = ON");

				w.Flush();
			}

			string arguments = @"/i:sysoc.inf /x /q /w /u:%temp%\" + Path.GetFileName(fileName);

			_log.DebugFormat("Executing Installer: " + arguments);

			return Process.Start("sysocmgr", arguments);
		}
	}
}