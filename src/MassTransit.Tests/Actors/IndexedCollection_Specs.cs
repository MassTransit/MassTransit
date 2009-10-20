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
namespace MassTransit.Tests.Actors
{
	using System.Linq;
	using NUnit.Framework;
	using Util;

	[TestFixture]
	public class An_indexed_collection
	{
		[Test]
		public void Should_use_indices_to_improve_query_performance()
		{
			IndexedCollection<IndexedClass> collection = new IndexedCollection<IndexedClass>();

			collection.Add(new IndexedClass {Name = "Chris"});
			collection.Add(new IndexedClass {Name = "David"});
			collection.Add(new IndexedClass {Name = "Jason"});
			collection.Add(new IndexedClass {Name = "Matt"});
			collection.Add(new IndexedClass {Name = "Terry"});
			collection.Add(new IndexedClass {Name = "Zach"});


			var result = collection.Where(x => x.Name == "Matt").First();

			Assert.AreEqual("Matt", result.Name);
		}
	}

	public class IndexedClass
	{
		[Indexed]
		public string Name { get; set; }
	}
}