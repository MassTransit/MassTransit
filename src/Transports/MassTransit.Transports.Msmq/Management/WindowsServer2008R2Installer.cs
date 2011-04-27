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
	using System;
	using System.Diagnostics;

	public class WindowsServer2008R2Installer :
		MsmqInstaller
	{
		const string Arguments = "-install MSMQ-Services MSMQ-Server MSMQ-Multicasting";
		const string ServerManager = "ServerManagerCmd.exe";

		public Process Install()
		{
			var psi = new ProcessStartInfo(ServerManager);
			psi.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
			psi.Arguments = Arguments;
			psi.UseShellExecute = false;

			return Process.Start(psi);
		}
	}
}