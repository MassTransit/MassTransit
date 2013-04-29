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
namespace MassTransit.Util
{
	using System;
	using System.Collections.Generic;
	using Magnum.Threading;

	/// <summary>
	///   A multiple-readers, single-writer, disposable, list implementation.
	/// </summary>
	/// <typeparam name = "T"></typeparam>
	public class RegistrationList<T> :
		IDisposable
	{
		volatile bool _disposed;
		ReaderWriterLockedObject<List<T>> _items = new ReaderWriterLockedObject<List<T>>(new List<T>());

		public void Dispose()
		{
			Dispose(true);
		}

		public UnregisterAction Register(T item)
		{
			_items.WriteLock(x => x.Insert(0, item));

			return () =>
				{
					bool removed = false;

					_items.WriteLock(x => { removed = x.Remove(item); });

					return removed;
				};
		}

		public void Each(Action<T> action)
		{
			_items.ReadLock(x => x.ForEach(action));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_items != null)
				_items.Dispose();

			_items = null;
			_disposed = true;
		}
	}
}