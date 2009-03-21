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
namespace MassTransit.Services.Subscriptions.Client
{
	using Magnum;
	using Messages;

	public class ClientSubscriptionInfoMapper :
		Mapper<SubscriptionInformation, ClientSubscriptionInformation>
	{
		public ClientSubscriptionInfoMapper()
		{
			From(x => x.MessageName).To(y => y.MessageName);
			From(x => x.EndpointUri).To(y => y.EndpointUri);
			From(x => x.CorrelationId).To(y => y.CorrelationId);
			From(x => x.SubscriptionId).To(y => y.SubscriptionId);
		}
	}
}