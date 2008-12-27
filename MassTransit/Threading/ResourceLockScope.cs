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
namespace MassTransit.Threading
{
	using System;

	public class ResourceLockScope<T> :
		IResourceLockScope<T>
	{
		private readonly Action _releaseFunction;
		private readonly T _resource;
		private volatile bool _disposed;
		private volatile bool _released;

		public ResourceLockScope(T resource, Action releaseFunction)
		{
			_resource = resource;
			_releaseFunction = releaseFunction;

			_released = false;
		}

		public T Resource
		{
			get
			{
				if (_disposed) throw new ObjectDisposedException("The object has been disposed");
				if (_released) throw new ResourceLockException("The resource is no longer locked and cannot be referenced");
				return _resource;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Release()
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (_released)
				return;

			_releaseFunction();
			_released = true;
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Release();
			}
			_disposed = true;
		}

		~ResourceLockScope()
		{
			Dispose(false);
		}
	}
}