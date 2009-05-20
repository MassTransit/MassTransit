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
namespace MassTransit.Tests.Saga.Locator
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Magnum;
	using MassTransit.Saga;
	using NUnit.Framework;

	[TestFixture]
	public class SagaExpression_Specs
	{
		private Expression<Func<TSaga, bool>> BuildExpression<TSaga, TMessage>(Expression<Func<TSaga, TMessage, bool>> expression, TMessage message)
			where TSaga : ISaga
			where TMessage : class, CorrelatedBy<Guid>
		{
			return x => x.CorrelationId == message.CorrelationId;
		}

		[Test]
		public void The_saga_expression_should_be_converted_down_to_a_saga_only_filter()
		{
			var message = new InitiateSimpleSaga { CorrelationId = CombGuid.Generate() };

			Expression<Func<SimpleSaga, InitiateSimpleSaga, bool>> selector = (s, m) => s.CorrelationId == m.CorrelationId;

			var filter = BuildExpression(selector, message);

			Trace.WriteLine(filter.ToString());
		}
	}
}