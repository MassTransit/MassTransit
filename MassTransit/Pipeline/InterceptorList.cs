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
namespace MassTransit.Pipeline
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.Threading;

	public class InterceptorList<T> :
		IInterceptorList<T>
	{
		private volatile bool _disposed;
		private ReaderWriterLockedObject<List<T>> _interceptors = new ReaderWriterLockedObject<List<T>>(new List<T>());

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Func<bool> Register(T interceptor)
		{
			_interceptors.WriteLock(x => x.Add(interceptor));

			return () =>
				{
					bool removed = false;

					_interceptors.WriteLock(x => { removed = x.Remove(interceptor); });

					return removed;
				};
		}

		public void Unregister(T interceptor)
		{
			_interceptors.WriteLock(x => x.Remove(interceptor));
		}

		public void ForEach(Action<T> action)
		{
			_interceptors.ReadLock(x => x.ForEach(action));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_interceptors != null)
				_interceptors.Dispose();

			_interceptors = null;
			_disposed = true;
		}

		~InterceptorList()
		{
			Dispose(false);
		}
	}
}