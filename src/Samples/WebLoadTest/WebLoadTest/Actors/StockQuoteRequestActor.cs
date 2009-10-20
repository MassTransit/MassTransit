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
namespace WebLoadTest.Actors
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using System.Threading;
	using System.Web;
	using Magnum.Actors;
	using Magnum.StateMachine;
	using MassTransit;
	using MassTransit.Actors;

	public class StockQuoteRequestActor :
		StateDrivenActor<StockQuoteRequestActor>,
		AsyncHttpActor
	{
		private HttpContext _context;
		private readonly StringBuilder _body = new StringBuilder(1024);

		static StockQuoteRequestActor()
		{
			Define(() =>
				{
					Correlate(InitiateRequest).By((actor, message) => actor.CorrelationId == message.RequestId);
					Correlate(QuoteReceived).By((actor, message) => actor.Symbol == message.Symbol);

					Initially(
						When(InitiateRequest)
							.Then((actor, message) =>
								{
									actor.Stopwatch = Stopwatch.StartNew();
									actor.Symbol = message.Symbol;

									actor.Bus.Publish(new RequestStockQuoteImpl
										{
											RequestId = message.RequestId, Symbol = message.Symbol
										}, x => x.SendResponseTo(actor.Bus.Endpoint));
								})
							.TransitionTo(Active));

					During(Active,
						When(QuoteReceived)
							.Then((actor, message) =>
								{
									actor.Bid = message.Bid;
									actor.Ask = message.Ask;
									actor.Last = message.Last;

									actor.Stopwatch.Stop();
								})
							.TransitionTo(Completed));

					Anytime(When(Completed.Enter).Then(actor => actor.Complete()));

					Anytime(When(InitiateRequest).Then(x => { }));
				});
		}

		protected decimal Bid { get; set; }
		protected decimal Ask { get; set; }
		protected decimal Last { get; set; }

		protected Stopwatch Stopwatch { get; set; }

		protected string Symbol { get; set; }

		public static State Initial { get; set; }
		public static State Completed { get; set; }
		public static State Active { get; set; }

		public static Event<InitiateStockQuoteRequest> InitiateRequest { get; set; }
		public static Event<StockQuoteReceived> QuoteReceived { get; set; }

		public StockQuoteRequestActor(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public StockQuoteRequestActor(Guid id, HttpContext context, AsyncCallback callback, object extraData)
			: base(id, callback, extraData)
		{
			_context = context;
			_body.AppendLine("Actor created on thread: " + Thread.CurrentThread.ManagedThreadId);
		}

		public IAsyncResult BeginAction(HttpContext context, AsyncCallback callback, object state)
		{
			_body.AppendLine("Begin called on thread: " + Thread.CurrentThread.ManagedThreadId);

			return this;
		}

		private void Complete()
		{
			_body.AppendLine("Request duration: " + Stopwatch.ElapsedMilliseconds + "ms");
			_body.AppendLine("Bid: " + Bid);
			_body.AppendLine("Ask: " + Ask);
			_body.AppendLine("Last: " + Last);
			_body.AppendLine("Callback executed on thread: " + Thread.CurrentThread.ManagedThreadId);

			_context.Response.ContentType = "text/plain";
			_context.Response.Write(_body.ToString());

			SetAsCompleted();
		}
	}
}