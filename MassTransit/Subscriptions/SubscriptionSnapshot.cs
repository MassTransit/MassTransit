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
namespace MassTransit.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using Util;

	public class SubscriptionSnapshot
	{
		private readonly HashSet<Tuple<Type, string>> _correlatedTypes = new HashSet<Tuple<Type, string>>();
		private readonly HashSet<Type> _openTypes = new HashSet<Type>();

		public void UpdateFromSnapshot(SubscriptionSnapshot update, Action<Type, string> added, Action<Type, string> removed)
		{
			AddNewOpenTypes(update, added);
			RemoveOldOpenTypes(update, removed);

			AddNewCorrelatedTypes(update, added);
			RemoveOldCorrelatedTypes(update, removed);
		}

		private void AddNewOpenTypes(SubscriptionSnapshot update, Action<Type, string> added)
		{
			HashSet<Type> hashSet = new HashSet<Type>(update._openTypes);

			hashSet.ExceptWith(_openTypes);

			foreach (Type type in hashSet)
			{
				_openTypes.Add(type);
				added(type, string.Empty);
			}
		}

		private void AddNewCorrelatedTypes(SubscriptionSnapshot update, Action<Type, string> added)
		{
			HashSet<Tuple<Type, string>> hashSet = new HashSet<Tuple<Type, string>>(update._correlatedTypes);

			hashSet.ExceptWith(_correlatedTypes);

			foreach (Tuple<Type, string> type in hashSet)
			{
				_correlatedTypes.Add(type);
				added(type.Key, type.Value);
			}
		}

		private void RemoveOldOpenTypes(SubscriptionSnapshot update, Action<Type, string> removed)
		{
			HashSet<Type> hashSet = new HashSet<Type>(_openTypes);

			hashSet.ExceptWith(update._openTypes);

			foreach (Type type in hashSet)
			{
				_openTypes.Remove(type);
				removed(type, string.Empty);
			}
		}

		private void RemoveOldCorrelatedTypes(SubscriptionSnapshot update, Action<Type, string> removed)
		{
			HashSet<Tuple<Type, string>> hashSet = new HashSet<Tuple<Type, string>>(_correlatedTypes);

			hashSet.ExceptWith(update._correlatedTypes);

			foreach (Tuple<Type, string> type in hashSet)
			{
				_correlatedTypes.Remove(type);
				removed(type.Key, type.Value);
			}
		}

		public void Add(Type messageType)
		{
			if (_openTypes.Contains(messageType))
				return;

			_openTypes.Add(messageType);
		}

		public void Add(Type messageType, string correlationId)
		{
			Tuple<Type, string> item = new Tuple<Type, string>(messageType, correlationId);

			if (_correlatedTypes.Contains(item))
				return;

			_correlatedTypes.Add(item);
		}
	}
}