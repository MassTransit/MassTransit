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
	using EndpointConfigurators;
	using Serialization;

	public static class SerializerConfigurationExtensions
	{
		public static T UseDotNetXmlSerializer<T>(this T configurator)
			where T : EndpointFactoryConfigurator
		{
			configurator.SetDefaultSerializer<DotNotXmlMessageSerializer>();

			return configurator;
		}

		public static T UseJsonSerializer<T>(this T configurator)
			where T : EndpointFactoryConfigurator
		{
			configurator.SetDefaultSerializer<JsonMessageSerializer>();

			return configurator;
		}

		public static T UseXmlSerializer<T>(this T configurator)
			where T : EndpointFactoryConfigurator
		{
			configurator.SetDefaultSerializer<XmlMessageSerializer>();

			return configurator;
		}

		public static T UseBinarySerializer<T>(this T configurator)
			where T : EndpointFactoryConfigurator
		{
			configurator.SetDefaultSerializer<BinaryMessageSerializer>();

			return configurator;
		}
	}
}