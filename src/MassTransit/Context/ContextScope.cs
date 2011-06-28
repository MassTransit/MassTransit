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
namespace MassTransit.Context
{
	using System;
	using System.Diagnostics;

	public class ContextScope :
		IDisposable
	{
		readonly Action _restore;

		ContextScope(Action restore)
		{
			_restore = restore;
		}

		public void Dispose()
		{
			_restore();
		}

		public static IDisposable FromSendContext<T>(ISendContext<T> context)
			where T : class
		{
			ISendContext previousContext = ContextStorage.CurrentSendContext;

			Debug.Assert(!ReferenceEquals(previousContext, context));

			var currentConsumeContext = ContextStorage.CurrentConsumeContext;
			if (currentConsumeContext != null)
			{
				var receiveContext = currentConsumeContext as IReceiveContext;
				if (receiveContext != null)
					context.SetReceiveContext(receiveContext);
				else if (currentConsumeContext.BaseContext != null)
				{
					context.SetReceiveContext(currentConsumeContext.BaseContext);
				}
			}
			ContextStorage.CurrentSendContext = context;

			return new ContextScope(() => ContextStorage.CurrentSendContext = previousContext);
		}

		public static IDisposable FromPublishContext<T>(IBusPublishContext<T> context)
			where T : class
		{
			return FromSendContext(context);
		}

		public static IDisposable FromReceiveContext(IReceiveContext context)
		{
			ContextStorage.CurrentConsumeContext = context;

			return new ContextScope(() => ContextStorage.CurrentConsumeContext = null);
		}

		public static IDisposable FromConsumeContext<T>(IConsumeContext<T> context)
			where T : class
		{
			IConsumeContext previousContext = ContextStorage.CurrentConsumeContext;

			Debug.Assert(!ReferenceEquals(previousContext, context));

			ContextStorage.CurrentConsumeContext = context;

			return new ContextScope(() => ContextStorage.CurrentConsumeContext = previousContext);
		}
	}
}