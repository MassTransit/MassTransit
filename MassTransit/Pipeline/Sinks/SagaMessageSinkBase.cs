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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Interceptors;
	using Saga;

	public abstract class SagaMessageSinkBase<TComponent, TMessage> :
		IMessageSink<TMessage>
		where TComponent : class, Consumes<TMessage>.All, ISaga
		where TMessage : class, CorrelatedBy<Guid>
	{
		private volatile bool _disposed;

		protected SagaMessageSinkBase(IInterceptorContext context, IServiceBus bus, ISagaRepository<TComponent> repository)
		{
			Builder = context.Builder;
			Bus = bus;
			Repository = repository;
		}

		protected IObjectBuilder Builder { get; private set; }
		protected IServiceBus Bus { get; private set; }
		protected ISagaRepository<TComponent> Repository { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public abstract IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message);

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Repository.Dispose();
				Repository = null;
			}

			_disposed = true;
		}

		~SagaMessageSinkBase()
		{
			Dispose(false);
		}
	}
}