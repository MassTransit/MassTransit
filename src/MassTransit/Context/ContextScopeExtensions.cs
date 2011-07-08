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
	using System;
	using Context;

	public static class ContextScopeExtensions
	{
		public static IDisposable CreateScope<T>(this IBusPublishContext<T> context)
			where T : class
		{
			return ContextScope.FromPublishContext(context);
		}

		public static IDisposable CreateScope<T>(this ISendContext<T> context)
			where T : class
		{
			return ContextScope.FromSendContext(context);
		}

		public static IDisposable CreateScope(this IReceiveContext context)
		{
			return ContextScope.FromReceiveContext(context);
		}

		public static IDisposable CreateScope<T>(this IConsumeContext<T> context)
			where T : class
		{
			return ContextScope.FromConsumeContext(context);
		}
	}
}