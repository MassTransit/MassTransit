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
namespace MassTransit.BusServiceConfigurators
{
	using System.Collections.Generic;
	using Builders;
	using BusConfigurators;
	using Configurators;

	public class CustomBusServiceConfigurator :
		BusBuilderConfigurator
	{
		readonly BusServiceConfigurator _configurator;

		public CustomBusServiceConfigurator(BusServiceConfigurator configurator)
		{
			_configurator = configurator;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (_configurator == null)
				yield return this.Failure("BusServiceConfigurator", "The bus service configurator cannot be null");
		}

		public BusBuilder Configure(BusBuilder builder)
		{
			builder.AddBusServiceConfigurator(_configurator);

			return builder;
		}
	}
}