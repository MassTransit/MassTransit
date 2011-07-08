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
	using NUnit.Framework;
	using Util;

	[TestFixture]
	public class Working_with_an_idempotent_hashtable
	{
		Uri a = new Uri("msmq://localhost/bob");
		Uri b = new Uri("msmq://localhost/bill");

		[Test]
		public void Should_not_fail_when_removing_non_existent_items()
		{
			var table = new IdempotentHashtable<Uri, object>();

			table.Add(a, new object());
			table.Add(b, new object());

			table.Remove(a);
			table.Remove(a);
			table.Remove(a);
			table.Remove(a);

			Assert.AreEqual(1, table.Count);
		}

		[Test]
		public void Should_only_add_duplicate_items_once()
		{
			var table = new IdempotentHashtable<Uri, object>();

			table.Add(a, new object());
			table.Add(a, new object());

			Assert.AreEqual(1, table.Count);

			table.Add(b, new object());
			Assert.AreEqual(2, table.Count);
		}
	}
}