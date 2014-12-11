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
namespace MassTransit.RabbitMqTransport.Configuration.Configurators
{
    using Builders;
    using MassTransit.Configurators;


    /// <summary>
	/// <para>A configurator for the connection factory builder, i.e.
	/// a thing that actually lets you configure the settings that will go
	/// into creating the connection factory.
	/// </para>
	/// <para>Digression: There are three turtles on the way down, then there's Atlas.</para>
	/// </summary>
	public interface ConnectionFactoryBuilderConfigurator :
		Configurator
	{
		/// <summary>
		/// Configure the connection factory builder.
		/// </summary>
		/// <param name="builder">The builder</param>
		/// <returns>An updated builder</returns>
		ConnectionFactoryBuilder Configure(ConnectionFactoryBuilder builder);
	}
}