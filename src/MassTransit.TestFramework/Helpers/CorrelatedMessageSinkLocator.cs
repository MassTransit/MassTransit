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
namespace MassTransit.TestFramework.Helpers
{
	using System;
	using Pipeline.Inspectors;
	using Pipeline.Sinks;

	public class CorrelatedMessageSinkLocator :
		PipelineInspectorBase<CorrelatedMessageSinkLocator>
	{
		readonly Type _keyType;
		readonly Func<string, bool> _match;
		readonly Type _messageType;

		public CorrelatedMessageSinkLocator(Type messageType, Type keyType, Func<string, bool> match)
		{
			_messageType = messageType;
			_keyType = keyType;
			_match = match;
		}

		public bool Success { get; private set; }

		public bool Inspect<T, TMessage, TKey>(CorrelatedMessageSinkRouter<T, TMessage, TKey> sink)
			where TMessage : class, CorrelatedBy<TKey>
			where T : class
		{
			if (typeof (TMessage) == _messageType && typeof (TKey) == _keyType)
			{
				string key = sink.CorrelationId.ToString();

				if (_match(key))
				{
					Success = true;
					return false;
				}
			}

			return true;
		}
	}
}