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
namespace MassTransit.Builders
{
	using System;
	using Magnum;
	using Util;

	public class ControlBusBuilderImpl :
		BusBuilder,
		IDisposable
	{
		readonly IEndpointCache _endpointCache;
		readonly BusSettings _settings;

		public ControlBusBuilderImpl([NotNull] BusSettings settings, [NotNull] IEndpointCache endpointCache)
		{
			Guard.AgainstNull(settings, "settings");
			Guard.AgainstNull(endpointCache, "endpointCache");

			_settings = settings;
			_endpointCache = endpointCache;
		}

		public IEndpointCache EndpointCache
		{
			get { return _endpointCache; }
		}

		public BusSettings Settings
		{
			get { return _settings; }
		}

		public IControlBus Build()
		{
		}

		public void Match<T>(Action<T> callback)
			where T : class, BusBuilder
		{
		}

		public void Dispose()
		{
		}
	}
}