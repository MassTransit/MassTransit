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
namespace MassTransit.Pipeline.Interceptors
{
	using System;

	public interface IInterceptorList<T> :
		IDisposable
	{
		/// <summary>
		/// Register a new interceptor at the front of the list. New intercepters are always
		/// inserted at the start of the list, so they should be added from least to most specific 
		/// order to avoid improper handling of message.
		/// </summary>
		/// <param name="interceptor">The intercept to insert in the list</param>
		/// <returns>The unregister function, which should be called to remove the interceptor</returns>
		Func<bool> Register(T interceptor);

		/// <summary>
		/// Enumerate the interceptors
		/// </summary>
		/// <param name="action"></param>
		void ForEach(Action<T> action);
	}
}