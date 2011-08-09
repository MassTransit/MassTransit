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
namespace MassTransit.SubscriptionConfigurators
{
	/// <summary>
	/// Configures the subscription coordinator that is used by the bus instance
	/// to publish messages
	/// </summary>
	public interface SubscriptionRouterConfigurator
	{
		/// <summary>
		/// Sets the network key for subscriptions so that subscription messages
		/// are tagged by network, preventing subscriptions from multiple unrelated applications
		/// from getting mixed up.
		/// </summary>
		/// <param name="network"></param>
		void SetNetwork(string network);
	}
}