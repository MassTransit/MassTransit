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
namespace MassTransit.Tests.Grid
{
	using System;

	public class ServiceGrid :
		Grid
	{
		private readonly IEndpointFactory _endpointFactory;
		private IServiceBus _bus;
		private IServiceBus _controlBus;


		public ServiceGrid(IEndpointFactory endpointFactory)
		{
			_endpointFactory = endpointFactory;
		}


		public bool IsHealthy
		{
			get { throw new NotImplementedException(); }
		}

		public void Start(IServiceBus bus)
		{
			_bus = bus;
			_controlBus = bus.ControlBus;

			_controlBus.Subscribe<NodeState>();
		}

		public void Execute<T>(T command)
		{
			// begin saga for this command

			// publish message

			// get message from worker that is processing this command

			// get completion message for this command
		}

		public void ConfigureService<T>(Action<T> action)
		{
			
		}
	}
}