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
namespace MassTransit.Testing.Scenarios
{
	using Transports;

	/// <summary>
	/// Adds the further feature onto <see cref="TestScenario"/> of having 
	/// an endpoint cache and an endpoint factory. This is useful if you are 
	/// testing that you can send messages to what can be considered 'endpoints' rather than
	/// just 'subscribers'. It's more in line to the usage scenario of request-reply or fire-and-forget
	/// rather than publish-subscribe.
	/// </summary>
	public interface EndpointTestScenario :
		TestScenario
	{
		/// <summary>
		/// Gets the endpoint cache. Use this instance to find the <see cref="IEndpoint"/>s.
		/// </summary>
		IEndpointCache EndpointCache { get; }
		
		/// <summary>
		/// Gets the endpoint factory.
		/// </summary>
		IEndpointFactory EndpointFactory { get; }
	}
}