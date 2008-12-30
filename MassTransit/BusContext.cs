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
namespace MassTransit
{
	using System;
	using System.Collections;
	using System.Web;
	using Internal;

	public static class BusContext
	{
		private static readonly object _busContextHashtableKey = new object();
		private static readonly IBusContext _current = new BusContextStorage();

		/// <summary>
		/// Gets the current data
		/// </summary>
		/// <value>The data.</value>
		public static IBusContext Current
		{
			get { return _current; }
		}

		/// <summary>
		/// Gets a value indicating whether running in the web context
		/// </summary>
		/// <value><c>true</c> if [running in web]; otherwise, <c>false</c>.</value>
		public static bool RunningInWeb
		{
			get { return HttpContext.Current != null; }
		}

		private class BusContextStorage :
			IBusContext
		{
			[ThreadStatic] private static Hashtable _threadLocalStorage;

			private static Hashtable ThreadLocalHashtable
			{
				get
				{
					if (!RunningInWeb)
					{
						return _threadLocalStorage ?? (_threadLocalStorage = new Hashtable());
					}

					Hashtable webHashtable = HttpContext.Current.Items[_busContextHashtableKey] as Hashtable;
					if (webHashtable == null)
					{
						HttpContext.Current.Items[_busContextHashtableKey] = webHashtable = new Hashtable();
					}
					return webHashtable;
				}
			}

			public TValue Retrieve<TValue>(object key, Func<TValue> valueProvider)
			{
				object existing = ThreadLocalHashtable[key];

				TValue value;
				if (existing == null)
				{
					value = valueProvider();
					ThreadLocalHashtable[key] = value;
				}
				else
				{
					value = (TValue) existing;
				}

				return value;
			}

			public object this[object key]
			{
				get { return ThreadLocalHashtable[key]; }
				set { ThreadLocalHashtable[key] = value; }
			}

			public void Clear()
			{
				ThreadLocalHashtable.Clear();
			}
		}
	}
}