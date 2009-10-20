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
namespace WebLoadTest
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Reflection;
	using System.Web;
	using Actors;
	using Magnum;
	using Magnum.Actors;
	using MassTransit;
	using MassTransit.Actors;
	using StructureMap.Pipeline;

	public class ActorHttpAsyncHandler<T> :
		IHttpAsyncHandler
		where T : StateDrivenActor<T>, AsyncHttpActor
	{
		private readonly IServiceBus _bus;
		private readonly IActorRepository<T> _actorRepository;

		public ActorHttpAsyncHandler(IServiceBus bus, IActorRepository<T> actorRepository)
		{
			_bus = bus;
			_actorRepository = actorRepository;
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException("This should not be called since we are an asynchronous handler");
		}

		public bool IsReusable
		{
			get { return true; }
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			Guid transactionId = CombGuid.Generate();

			T actor = (T)Activator.CreateInstance(typeof(T), new[] { transactionId, context, cb, extraData });

			_actorRepository.Add(actor);

			AsyncCallback callback = x =>
				{
					_actorRepository.Remove(actor);

					context.Response.Write("Repository Size: " + _actorRepository.Count());

					cb(x);
				};

			IAsyncResult asyncResult = actor.BeginAction(context, callback, extraData);
			
			_bus.Endpoint.Send(new InitiateStockQuoteRequestImpl {RequestId = transactionId, Symbol = "AAPL"});

			return asyncResult;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
		}
	}
}