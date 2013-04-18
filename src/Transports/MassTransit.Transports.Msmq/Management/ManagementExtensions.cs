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
namespace MassTransit
{
    using System;
    using BusConfigurators;
	using Transports.Msmq.Management;

	public static class ManagementExtensions
	{
		/// <summary>
		/// Will force an installation of MSMQ if it is not installed
		/// </summary>
		/// <param name="configurator"></param>
		[Obsolete("use the method inside UseMsmq instead")]
        public static void VerifyMsmqConfiguration(this ServiceBusConfigurator configurator)
		{
			var management = new MsmqManagement();

			if (!management.IsInstalled())
			{
				management.Install();
			}

			management.Start();
		}

        public static void VerifyMsmqConfiguration(this MsmqConfigurator configurator)
		{
			var management = new MsmqManagement();

			if (!management.IsInstalled())
			{
				management.Install();
			}

			management.Start();
		}


		/// <summary>
		/// This method will verify that the MS DTC is installed and properly configured. If
		/// the configuration is invalid or the MS DTC is not installed, it will be installed,
		/// configured, and/or started.
		/// </summary>
		/// <param name="configurator"></param>
		public static void VerifyMsDtcConfiguration(this ServiceBusConfigurator configurator)
		{
			var management = new MsDtcManagement();

			management.VerifyConfiguration(true, true);
			management.VerifyRunning(true);
		}
	}
}