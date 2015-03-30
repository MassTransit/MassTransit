// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Sending_ordered_notifications_from_a_receive_endpoint
    {
        [Test]
        public async void Should_remain_in_order()
        {
            var values = new List<int>();

            var observable = new AsyncObservable<List<int>>();

            ObserverHandle observer = observable.Connect(values);

            for (int i = 0; i < 10000; i++)
            {
                int index = i;
                observable.Notify(async x => x.Add(index));
            }

            await observer.Disconnect();

            Assert.AreEqual(10000, values.Count);


            Assert.IsTrue(values.SequenceEqual(Enumerable.Range(0, 10000).Select(x => x).ToList()));
        }
    }
}