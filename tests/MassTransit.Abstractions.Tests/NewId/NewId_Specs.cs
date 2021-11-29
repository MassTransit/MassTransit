namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using NewIdProviders;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_newid_generator
    {
        [Test]
        public void Should_be_able_to_determine_equal_ids()
        {
            var id1 = new NewId("fc070000-9565-3668-e000-08d5893343c6");
            var id2 = new NewId("fc070000-9565-3668-e000-08d5893343c6");

            Assert.IsTrue(id1 == id2);
        }

        [Test]
        public void Should_be_able_to_determine_greater_id()
        {
            var lowerId = new NewId("fc070000-9565-3668-e000-08d5893343c6");
            var greaterId = new NewId("fc070000-9565-3668-9180-08d589338b38");

            Assert.IsTrue(lowerId < greaterId);
        }

        [Test]
        public void Should_be_able_to_determine_lower_id()
        {
            var lowerId = new NewId("fc070000-9565-3668-e000-08d5893343c6");
            var greaterId = new NewId("fc070000-9565-3668-9180-08d589338b38");

            Assert.IsFalse(lowerId > greaterId);
        }

        [Test]
        [Explicit]
        public void Should_be_able_to_extract_timestamp()
        {
            var now = DateTime.UtcNow;
            var id = NewId.Next();

            var timestamp = id.Timestamp;

            Console.WriteLine("Now: {0}, Timestamp: {1}", now, timestamp);

            var difference = timestamp - now;
            if (difference < TimeSpan.Zero)
                difference = difference.Negate();

            Assert.LessOrEqual(difference, TimeSpan.FromMinutes(1));
        }

        [Test]
        [Explicit]
        public void Should_be_able_to_extract_timestamp_with_process_id()
        {
            var now = DateTime.UtcNow;
            NewId.SetProcessIdProvider(new CurrentProcessIdProvider());
            var id = NewId.Next();

            var timestamp = id.Timestamp;

            Console.WriteLine("Now: {0}, Timestamp: {1}", now, timestamp);

            var difference = timestamp - now;
            if (difference < TimeSpan.Zero)
                difference = difference.Negate();

            Assert.LessOrEqual(difference, TimeSpan.FromMinutes(1));
        }

        [Test]
        [Explicit]
        public void Should_be_completely_thread_safe_to_avoid_duplicates()
        {
            NewId.Next();

            var timer = Stopwatch.StartNew();

            var threadCount = 20;

            var loopCount = 1024 * 1024;

            var limit = loopCount * threadCount;

            var ids = new NewId[limit];

            ParallelEnumerable
                .Range(0, limit)
                .WithDegreeOfParallelism(8)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .ForAll(x =>
                {
                    ids[x] = NewId.Next();
                });

            timer.Stop();

            Console.WriteLine("Generated {0} ids in {1}ms ({2}/ms)", limit, timer.ElapsedMilliseconds,
                limit / timer.ElapsedMilliseconds);

            Console.WriteLine("Distinct: {0}", ids.Distinct().Count());

            IGrouping<NewId, NewId>[] duplicates = ids.GroupBy(x => x).Where(x => x.Count() > 1).ToArray();

            Console.WriteLine("Duplicates: {0}", duplicates.Count());

            foreach (IGrouping<NewId, NewId> newId in duplicates)
                Console.WriteLine("{0} {1}", newId.Key, newId.Count());
        }

        [Test]
        [Explicit]
        public void Should_be_fast_and_friendly()
        {
            NewId.Next();


            var limit = 1000000;

            var ids = new NewId[limit];

            Parallel.For(0, limit, x => ids[x] = NewId.Next());

            var timer = Stopwatch.StartNew();

            Parallel.For(0, limit, x => ids[x] = NewId.Next());

            timer.Stop();

            Console.WriteLine("Generated {0} ids in {1}ms ({2}/ms)", limit, timer.ElapsedMilliseconds,
                limit / timer.ElapsedMilliseconds);

            Console.WriteLine("Distinct: {0}", ids.Distinct().Count());

            IGrouping<NewId, NewId>[] duplicates = ids.GroupBy(x => x).Where(x => x.Count() > 1).ToArray();

            Console.WriteLine("Duplicates: {0}", duplicates.Count());

            foreach (IGrouping<NewId, NewId> newId in duplicates)
                Console.WriteLine("{0} {1}", newId.Key, newId.Count());
        }

        [Test]
        public void Should_generate_sequential_ids_quickly()
        {
            NewId.SetTickProvider(new StopwatchTickProvider());
            NewId.Next();

            var limit = 10;

            var ids = new NewId[limit];
            for (var i = 0; i < limit; i++)
                ids[i] = NewId.Next();

            for (var i = 0; i < limit - 1; i++)
            {
                Assert.AreNotEqual(ids[i], ids[i + 1]);
                Console.WriteLine(ids[i]);
            }
        }

        [Test]
        [Explicit]
        public void Should_generate_unique_identifiers_with_each_invocation()
        {
            NewId.Next();

            var timer = Stopwatch.StartNew();

            var limit = 1024 * 1024;

            var ids = new NewId[limit];
            for (var i = 0; i < limit; i++)
                ids[i] = NewId.Next();

            timer.Stop();

            for (var i = 0; i < limit - 1; i++)
            {
                Assert.AreNotEqual(ids[i], ids[i + 1]);
                var end = ids[i].ToString().Substring(32, 4);
                if (end == "0000")
                    Console.WriteLine("{0}", ids[i].ToString());
            }

            Console.WriteLine("Generated {0} ids in {1}ms ({2}/ms)", limit, timer.ElapsedMilliseconds,
                limit / timer.ElapsedMilliseconds);
        }
    }
}
