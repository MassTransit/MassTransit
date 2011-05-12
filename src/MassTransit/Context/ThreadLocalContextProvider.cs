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
//	public class ThreadLocalContextProvider :
//		IContextProvider
//	{
//		[ThreadStatic]
//		static Hashtable _threadStorage;
//
//		readonly object _hashKey = new object();
//
//		static bool RunningInWeb
//		{
//			get { return HttpContext.Current != null; }
//		}
//
//		Hashtable ContextCache
//		{
//			get
//			{
//				if (!RunningInWeb)
//				{
//					return _threadStorage ?? (_threadStorage = CreateContext());
//				}
//
//				var hashtable = HttpContext.Current.Items[_hashKey] as Hashtable;
//				if (hashtable == null)
//				{
//					HttpContext.Current.Items[_hashKey] = hashtable = CreateContext();
//				}
//				return hashtable;
//			}
//		}
//
//		public TContext ReceiveContext<TContext>(Action<TContext> contextAction)
//			where TContext : class, IReceiveContext
//		{
//			var context = Retrieve("ReceiveContext", () => new ConsumeContext());
//
//			contextAction(context);
//
//			return (TContext) context;
//		}
//
//		public TContext SendContext<TContext, TMessage>(TMessage message, Action<TContext> contextAction)
//			where TContext : ISendContext<TMessage> where TMessage : class
//		{
//			throw new NotImplementedException();
//		}
//
//		public TResult Context<TContext, TResult>(Func<TContext, TResult> accessor)
//		{
//			var context = Retrieve<TContext>();
//
//			return accessor(context);
//		}
//
//		public void Context<TContext, TMessage>(Action<TContext> action)
//			where TContext : ISendContext<TMessage>
//			where TMessage : class
//		{
//		}
//
//		public void Context<TContext>(Action<TContext> action)
//		{
//			var context = Retrieve<TContext>();
//
//			action(context);
//		}
//
//		TValue Retrieve<TValue>(string key, Func<TValue> valueProvider)
//		{
//			Hashtable cache = ContextCache;
//
//			if (cache.ContainsKey(key))
//				return (TValue)cache[key];
//
//			TValue value = valueProvider();
//
//			cache[key] = value;
//
//			return value;
//		}
//
//		static Hashtable CreateContext()
//		{
//			var hashtable = new Hashtable();
//			
//			return hashtable;
//		}
//	}
}