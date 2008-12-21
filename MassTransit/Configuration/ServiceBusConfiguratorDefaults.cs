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
namespace MassTransit.Configuration
{
	using System;
	using Magnum.Common.DateTimeExtensions;

	public class ServiceBusConfiguratorDefaults :
		IServiceBusConfiguratorDefaults
	{
		private IObjectBuilder _objectBuilder;
		private TimeSpan _receiveTimeout;
		private int _receiveThreadLimit;
		private int _threadLimit;

		public ServiceBusConfiguratorDefaults()
		{
			_receiveTimeout = 3.Seconds();
		}

		public void SetObjectBuilder(IObjectBuilder objectBuilder)
		{
			_objectBuilder = objectBuilder;
		}

		public void SetReceiveTimeout(TimeSpan receiveTimeout)
		{
			_receiveTimeout = receiveTimeout;
		}

		public void SetThreadLimit(int threadLimit)
		{
			_threadLimit = threadLimit;
		}

		public void SetReceiveThreadLimit(int receiveThreadLimit)
		{
			_receiveThreadLimit = receiveThreadLimit;
		}

		public void ApplyTo(IServiceBusConfigurator configurator)
		{
			configurator.SetObjectBuilder(_objectBuilder);
			configurator.SetReceiveTimeout(_receiveTimeout);
			configurator.SetThreadLimit(_threadLimit);
			configurator.SetReceiveThreadLimit(_receiveThreadLimit);
		}
	}
}