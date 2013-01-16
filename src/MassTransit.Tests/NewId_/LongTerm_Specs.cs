// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.NewId_
{
    using System;
    using System.Data.SqlTypes;
    using NUnit.Framework;
    using NewIdProviders;


    [TestFixture]
    public class Generating_ids_over_time
    {
        [Test]
        public void Should_keep_them_ordered_for_sql_server()
        {
            var generator = new NewIdGenerator(new TimeLapseTickProvider(), new NetworkAddressWorkerIdProvider());
            generator.Next();

            int limit = 1024;

            var ids = new NewId[limit];
            for (int i = 0; i < limit; i++)
                ids[i] = generator.Next();

            for (int i = 0; i < limit - 1; i++)
            {
                Assert.AreNotEqual(ids[i], ids[i + 1]);

                SqlGuid left = ids[i].ToGuid();
                SqlGuid right = ids[i + 1].ToGuid();
                Assert.Less(left, right);
                if (i % 128 == 0)
                    Console.WriteLine("Normal: {0} Sql: {1}", left, ids[i].ToSequentialGuid());
            }
        }


        class TimeLapseTickProvider :
            ITickProvider
        {
            TimeSpan _interval = TimeSpan.FromSeconds(2);
            DateTime _previous = DateTime.UtcNow;

            public long Ticks
            {
                get
                {
                    _previous = _previous + _interval;
                    _interval = TimeSpan.FromSeconds((long)_interval.TotalSeconds + 30);
                    return _previous.Ticks;
                }
            }
        }
    }
}