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

using MassTransit.Util;

namespace MassTransit.BusConfigurators
{
	using Builders;
	using Configurators;

	/// <summary>
	/// A thing that configures the thing that builds the bus.
	/// </summary>
	public interface BusBuilderConfigurator :
		Configurator
	{
		/// <summary>
		/// Pays the bus builder a visit and return a new builder
		/// that is correctly configured.
		/// </summary>
		/// <param name="builder">The bus builder</param>
		/// <returns>An updated builder.</returns>
		
		BusBuilder Configure( BusBuilder builder);
	}
}