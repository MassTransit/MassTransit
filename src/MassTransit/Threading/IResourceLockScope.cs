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

	public interface IResourceLockScope<T> :
		IDisposable
	{
		/// <summary>
		/// The resource for which the lock was acquired
		/// </summary>
		T Resource { get; }

		/// <summary>
		/// Releases the lock on the resource. Note this should only be called when
		/// the resource will no longer be accessed.
		/// </summary>
		void Release();
	}
}