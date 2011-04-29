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
namespace MassTransit.RequestResponse
{
	using System;

	public class ConditionalResponseActionBuilder<T>
		where T : class
	{
		private readonly Func<T, bool> _accept;
		private readonly RequestResponseScope _scope;

		public ConditionalResponseActionBuilder(RequestResponseScope scope, Func<T, bool> accept)
		{
			_scope = scope;
			_accept = accept;
		}

		public RequestResponseScope IsReceived(Action<T> action)
		{
			_scope.AddResponseAction(new ConditionalResponseAction<T>(_scope, _accept, action));

			return _scope;
		}
	}
}