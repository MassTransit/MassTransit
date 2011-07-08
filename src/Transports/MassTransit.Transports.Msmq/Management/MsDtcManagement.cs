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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Exceptions;
	using Magnum.Extensions;
	using Microsoft.Win32;

	/// <summary>
	/// Manages the configuration of the MS DTC, since this screws a lot of 
	/// people up a lot of the time when dealing with MSSQL and MSMQ. This is 
	/// in MSMQ since none of the other queue transports support the DTC.
	/// </summary>
	public class MsDtcManagement
	{
		readonly IEnumerable<string> _registryValues;

		public MsDtcManagement()
		{
			_registryValues = new[]
				{
					"NetworkDtcAccess",
					"NetworkDtcAccessOutbound",
					"NetworkDtcAccessTransactions",
					"XaTransactions"
				};
		}

		public void VerifyRunning(bool allowStart)
		{
			using (var service = new WindowsService("MSDTC"))
			{
				bool running = service.IsRunning();
				if (running)
					return;

				if (!allowStart)
					throw new InvalidOperationException("The MSDTC is not running and allowStart was not specified.");

				service.Start();
			}
		}

		public void VerifyConfiguration(bool allowChanges, bool allowInstall)
		{
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\MSDTC\Security", allowChanges);
				if (registryKey == null)
				{
					if (allowInstall)
					{
						Install();

						registryKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\MSDTC\Security", allowChanges);
						if (registryKey == null)
							throw new NotSupportedException("The MSDTC is not installed and could not be installed.");
					}
					else
						throw new NotSupportedException("The MSDTC is not installed.");
				}

				using (registryKey)
				{
					var incorrectValues = _registryValues
						.Select(key => new {Key = key, Value = (int) registryKey.GetValue(key)})
						.Where(x => x.Value == 0);

					if (!incorrectValues.Any())
						return;

					if (!allowChanges)
						throw new ConfigurationException("The MSDTC is not properly configured.");

					incorrectValues.Each(x => registryKey.SetValue(x.Key, 1, RegistryValueKind.DWord));

					Restart();
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new InvalidOperationException("The configuration could not be changed due to access permissions", ex);
			}
		}

		static void Restart()
		{
			using (var service = new WindowsService("MSDTC"))
			{
				service.Restart();
			}
		}

		static void Install()
		{
			Process process = Process.Start("MSDTC.EXE", "-install");
			if (process == null)
				throw new InvalidOperationException("Failed to start MSDTC.EXE for installation");

			using (process)
			{
				process.WaitForExit();
			}
		}
	}
}