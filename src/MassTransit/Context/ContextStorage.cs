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
	using System.Collections;
	using System.Web;

	/// <summary>
	/// The default context provider using thread local storage
	/// </summary>
	public static class ContextStorage
	{
		const string InboundContextKey = "InboundContext";
		const string OutboundContextKey = "OutboundContext";

		static readonly object _hashKey = new object();

		[ThreadStatic]
		static Hashtable _threadStorage;

		static Hashtable ContextCache
		{
			get
			{
				if (!RunningInWeb)
				{
					return _threadStorage ?? (_threadStorage = CreateContext());
				}

				var hashtable = HttpContext.Current.Items[_hashKey] as Hashtable;
				if (hashtable == null)
				{
					HttpContext.Current.Items[_hashKey] = hashtable = CreateContext();
				}

				return hashtable;
			}
		}

		static bool RunningInWeb
		{
			get { return HttpContext.Current != null; }
		}

		public static IDisposable CreateContextScope(IConsumeContext context)
		{
			var previousContext = Retrieve<IConsumeContext>(InboundContextKey);

			Store(InboundContextKey, context);

			return new PreviousContext<IConsumeContext>(InboundContextKey, previousContext);
		}

		public static IConsumeContext Context()
		{
			var context = Retrieve<IConsumeContext>(InboundContextKey);
			if (context == null)
				throw new InvalidOperationException("No consumer context was found");

			return context;
		}

		public static void Context(Action<IConsumeContext> contextCallback)
		{
			var context = Retrieve<IConsumeContext>(InboundContextKey);
			if (context == null)
				throw new InvalidOperationException("No consumer context was found");

			contextCallback(context);
		}

		public static TResult Context<TResult>(Func<IConsumeContext, TResult> contextCallback)
		{
			var context = Retrieve<IConsumeContext>(InboundContextKey);
			if (context == null)
				throw new InvalidOperationException("No consumer context was found");

			return contextCallback(context);
		}

		public static IDisposable CreateContextScope<T>(IBusPublishContext<T> context) 
			where T : class
		{
			var previousContext = Retrieve<ISendContext>(OutboundContextKey);

			Store(OutboundContextKey, context);

			return new PreviousContext<ISendContext>(OutboundContextKey, previousContext);
		}	
		
		public static IDisposable CreateContextScope<T>(ISendContext<T> context) 
			where T : class
		{
			var previousContext = Retrieve<ISendContext>(OutboundContextKey);

			Store(OutboundContextKey, context);

			return new PreviousContext<ISendContext>(OutboundContextKey, previousContext);
		}

		internal static void Store<TValue>(string key, TValue value)
		{
			Hashtable cache = ContextCache;

			cache[key] = value;
		}

		static TValue Retrieve<TValue>(string key)
		{
			Hashtable cache = ContextCache;

			if (cache.ContainsKey(key))
				return (TValue) cache[key];

			return default(TValue);
		}

		static Hashtable CreateContext()
		{
			var hashtable = new Hashtable();

			hashtable[InboundContextKey] = new InvalidConsumeContext();
			hashtable[OutboundContextKey] = new InvalidSendContext();

			return hashtable;
		}
	}
}