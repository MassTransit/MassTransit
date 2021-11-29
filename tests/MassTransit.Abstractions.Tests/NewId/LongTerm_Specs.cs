namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Data.SqlTypes;
    using NewIdProviders;
    using NUnit.Framework;


    [TestFixture]
    public class Generating_ids_over_time
    {
        [Test]
        public void Should_keep_them_ordered_for_sql_server()
        {
            var generator = new NewIdGenerator(new TimeLapseTickProvider(), new NetworkAddressWorkerIdProvider());
            generator.Next();

            var limit = 1024;

            var ids = new NewId[limit];
            for (var i = 0; i < limit; i++)
                ids[i] = generator.Next();

            for (var i = 0; i < limit - 1; i++)
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
