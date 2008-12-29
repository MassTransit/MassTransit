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
namespace MassTransit.Pipeline.Configuration
{
	using System;
	using Interceptors;

	public class MessagePipelineConfiguratorBase :
		IDisposable
	{
		private volatile bool _disposed;

		protected InterceptorList<IPipelineInterceptor> _interceptors = new InterceptorList<IPipelineInterceptor>();

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public UnregisterAction RegisterInterceptor(IPipelineInterceptor interceptor)
		{
			return _interceptors.Register(interceptor);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing || _disposed) return;

			if (_interceptors != null)
				_interceptors.Dispose();

			_interceptors = null;
			_disposed = true;
		}

		~MessagePipelineConfiguratorBase()
		{
			Dispose(false);
		}
	}
}