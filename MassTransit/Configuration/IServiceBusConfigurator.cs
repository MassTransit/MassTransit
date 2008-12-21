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

	/// <summary>
	/// Enables the configuration of the service bus when it is being created
	/// </summary>
	public interface IServiceBusConfigurator
	{
		/// <summary>
		/// Specify the IObjectBuilder to use when creating objects
		/// </summary>
		/// <param name="objectBuilder"></param>
		void SetObjectBuilder(IObjectBuilder objectBuilder);

		/// <summary>
		/// Specify the endpoint from which messages should be read
		/// </summary>
		/// <param name="uriString">The uri of the endpoint</param>
		void ReceiveFrom(string uriString);

		/// <summary>
		/// Specify the endpoint from which messages should be read
		/// </summary>
		/// <param name="uri">The uri of the endpoint</param>
		void ReceiveFrom(Uri uri);

		/// <summary>
		/// Specify the endpoint where errors should be sent
		/// </summary>
		/// <param name="uriString">The Uri of the endpoint</param>
		void SendErrorsTo(string uriString);

		/// <summary>
		/// Specify the endpoint where errors should be sent
		/// </summary>
		/// <param name="uri">The Uri of the endpoint</param>
		void SendErrorsTo(Uri uri);

		/// <summary>
		/// Set the maximum number of threads that can be used to consume messages on the bus
		/// </summary>
		/// <param name="threadLimit"></param>
		void SetThreadLimit(int threadLimit);

		/// <summary>
		/// Enable the automatic subscription feature to subscribe all consumers found using the object builder
		/// </summary>
		void EnableAutoSubscribe();

		/// <summary>
		/// Do not start the ServiceBus when it is created
		/// </summary>
		void DisableAutoStart();

		/// <summary>
		/// Configure a service for use by the service bus
		/// </summary>
		/// <typeparam name="TServiceConfigurator"></typeparam>
		/// <param name="action"></param>
		void ConfigureService<TServiceConfigurator>(Action<TServiceConfigurator> action) where TServiceConfigurator : IServiceConfigurator;
	}
}