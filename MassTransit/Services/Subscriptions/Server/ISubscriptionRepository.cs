/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.Subscriptions
{
	using System;
	using System.Collections.Generic;

	public interface ISubscriptionRepository :
		IDisposable
	{
		/// <summary>
		/// Add a new subscription to the repository for storage
		/// </summary>
		void Save(Subscription subscription);

		/// <summary>
		/// Remove a subscription from the repository
		/// </summary>
		void Remove(Subscription subscription);

		/// <summary>
		/// Return a list of subscriptions stored in the repository
		/// </summary>
		/// <returns></returns>
		IEnumerable<Subscription> List();
	}
}