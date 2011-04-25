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
namespace MassTransit.Tests.Internal
{
	using System;
	using MassTransit.Internal;
	using NUnit.Framework;

	[TestFixture]
	public class IdempotentList_Specs
	{
		Uri a = new Uri("msmq://localhost/test");
		Uri b = new Uri("msmq://localhost/test");
		Uri c = new Uri("msmq://localhost/test2");

		[Test]
		public void Add()
		{
			var list = new IdempotentList<Uri>();
			list.Add(a);
			list.Add(b);

			Assert.AreEqual(1, list.Count);

			list.Add(c);
			Assert.AreEqual(2, list.Count);
		}

		[Test]
		public void Remove()
		{
			var list = new IdempotentList<Uri>();
			list.Add(a);
			list.Add(b);
			list.Add(c);
			list.Remove(a);
			Assert.AreEqual(1, list.Count);
		}
	}
}