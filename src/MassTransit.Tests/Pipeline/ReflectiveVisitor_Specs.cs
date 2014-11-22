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
namespace MassTransit.Tests.Pipeline
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using Magnum.Reflection;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;
	using TestFramework.Messages;


    [TestFixture]
	public class When_visiting_an_object_graph
	{
		[Test]
		public void Only_a_single_method_should_dispatch_the_information()
		{
			MyVisitor visitor = new MyVisitor();

			MyObjectGraph graph = new MyObjectGraph();

			graph.Accept(visitor);
		}
	}

	public class MyObjectGraph
	{
		private readonly TestCorrelatedConsumer<PingMessage, Guid> _correlated = new TestCorrelatedConsumer<PingMessage, Guid>(Guid.Empty);
		private readonly IndiscriminantConsumer<PingMessage> _ping = new IndiscriminantConsumer<PingMessage>();
		private readonly IndiscriminantConsumer<PongMessage> _pong = new IndiscriminantConsumer<PongMessage>();

		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this, () => visitor.Visit(_ping) && visitor.Visit(_pong) && visitor.Visit(_correlated));
		}
	}

	public interface IVisitor
	{
		bool Visit(object obj);
		bool Visit(object obj, Func<bool> action);
	}

	public class MyVisitor :
		ReflectiveVisitorBase<MyVisitor>,
		IVisitor
	{
		public MyVisitor()
			: base("Visit")
		{
		}

		public bool Visit(IndiscriminantConsumer<PingMessage> obj)
		{
			Trace.WriteLine("IndiscriminantConsumer<PingMessage>");

			return true;
		}

		public bool Visit<TMessage>(IndiscriminantConsumer<TMessage> obj)
			where TMessage : class
		{
			Trace.WriteLine("IndiscriminantConsumer<TMessage> - " + typeof (TMessage));

			return true;
		}

		public bool Visit<TMessage, TKey>(TestCorrelatedConsumer<TMessage, TKey> obj)
			where TMessage : class, CorrelatedBy<Guid>
		{
			Trace.WriteLine(string.Format("TestCorrelatedConsumer<TMessage,TKey> - {0}({1})", typeof (TMessage).FullName, typeof (TKey).FullName));

			return true;
		}

		public bool Visit(MyObjectGraph obj)
		{
			Trace.WriteLine("MyObjectGraph");

			return true;
		}
	}
}