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
	using System.Linq;
	using System.ServiceProcess;
	using log4net;
	using Magnum.Extensions;

	public class WindowsService :
		IDisposable
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (WindowsService));

		readonly TimeSpan _timeout;
		ServiceController _controller;

		public WindowsService(string serviceName)
		{
			_timeout = 30.Seconds();
			_controller = new ServiceController(serviceName);
		}

		public void Dispose()
		{
			_controller.Dispose();
			_controller = null;
		}

		public bool Start()
		{
			return ControlService(ServiceControllerStatus.Running, x => x.Start());
		}

		public bool Stop()
		{
			return ControlService(ServiceControllerStatus.Stopped, x => x.Stop());
		}

		public bool Restart()
		{
			Stop();

			return Start();
		}

		public bool IsStopped()
		{
			return IsServiceInStatus(ServiceControllerStatus.Stopped, ServiceControllerStatus.StopPending);
		}

		public bool IsRunning()
		{
			return IsServiceInStatus(ServiceControllerStatus.Running, ServiceControllerStatus.StartPending,
				ServiceControllerStatus.ContinuePending);
		}

		bool ControlService(ServiceControllerStatus status, Action<ServiceController> controlAction)
		{
			if (_controller.Status == status)
			{
				_log.DebugFormat("The {0} service is already in the requested state: {1}", _controller.ServiceName, status);
				return false;
			}

			_log.DebugFormat("Setting the {0} service to {1}", _controller.ServiceName, status);

			try
			{
				controlAction(_controller);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("The {0} service could not be set to {1}"
					.FormatWith(_controller.ServiceName, status), ex);
			}

			_controller.WaitForStatus(status, _timeout);
			if (_controller.Status == status)
			{
				_log.DebugFormat("The {0} service was set to {1} successfully", _controller.ServiceName, status);
			}
			else
			{
				throw new InvalidOperationException("A timeout occurred waiting for the {0} service to be {1}"
					.FormatWith(_controller.ServiceName, status));
			}

			return true;
		}

		bool IsServiceInStatus(params ServiceControllerStatus[] statuses)
		{
			ServiceControllerStatus serviceStatus = _controller.Status;

			return statuses.Contains(serviceStatus);
		}
	}
}