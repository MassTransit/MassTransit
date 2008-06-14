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
namespace MassTransit.Host2.Hosts
{
	using System;
	using System.Threading;
	using log4net;

	public class ConsoleHost
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (ConsoleHost));
		private readonly HostedEnvironment _environment;


		public ConsoleHost(HostedEnvironment environment)
		{
			_environment = environment;
		}

		public void Run()
		{
			_log.Debug("Starting up as a console application");

			ManualResetEvent serviceCompleted = new ManualResetEvent(false);
			ManualResetEvent terminateService = new ManualResetEvent(false);

			WaitHandle[] waitHandles = new WaitHandle[] {serviceCompleted, terminateService};

			_environment.Completed += delegate { serviceCompleted.Set(); };

			Console.CancelKeyPress += delegate
			                          	{
			                          		_log.Info("Control+C detected, exiting.");
			                          		terminateService.Set();
			                          	};

			_environment.Start();

			_log.InfoFormat("The service is running, press Control+C to exit.");

            _environment.Main();

			int result;
			while ((result = WaitHandle.WaitAny(waitHandles, TimeSpan.FromSeconds(30), true)) == WaitHandle.WaitTimeout)
			{
			}

			if (result == 0) // our service completed
			{
			}
			else if (result == 1 || result == 2) // the application service exited
			{
				_log.Info("Stopping the service");
				_environment.Stop();
			}
		}
	}
}