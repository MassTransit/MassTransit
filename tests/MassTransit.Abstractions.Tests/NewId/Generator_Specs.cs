namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Diagnostics;
    using NUnit.Framework;


    [TestFixture]
    public class When_generating_id
    {
        [Test]
        public void Should_generate_known_guid()
        {
            var expected = Guid.Parse("437f0b01-bf34-7d81-3cf0-74b719ec7596");
            var tickProvider = new MockTickProvider(8410219332513447152);
            var networkProvider = new MockNetworkProvider(BitConverter.GetBytes(6857996259202924925));
            var generator = new NewIdGenerator(tickProvider, networkProvider);

            for (var i = 0; i < 267; i++)
                generator.NextGuid();

            var guid = generator.NextGuid();

            Assert.That(guid, Is.EqualTo(expected));
        }

        [Test]
        public void Should_generate_known_guid_batch()
        {
            var exp = new string[] { "74b719ec-7596-3cf0-7d81-bf34437f0b01", "74b719ec-7596-3cf0-7d81-bf34437f0c01", "74b719ec-7596-3cf0-7d81-bf34437f0d01" };
            var tickProvider = new MockTickProvider(8410219332513447152);
            var networkProvider = new MockNetworkProvider(BitConverter.GetBytes(6857996259202924925));
            var generator = new NewIdGenerator(tickProvider, networkProvider);

            for (var i = 0; i < 267; i++)
                generator.NextGuid();

            var batch = new Guid[3];
            generator.NextSequentialGuid(batch, 0, batch.Length);

            for (var i = 0; i < exp.Length; i++)
            {
                var guid = Guid.Parse(exp[i]);
                Assert.That(batch[i], Is.EqualTo(guid));
            }
        }

        [Test]
        public void Should_generate_known_sequential_guid()
        {
            var expected = Guid.Parse("74b719ec-7596-3cf0-7d81-bf34437f0b01");
            var tickProvider = new MockTickProvider(8410219332513447152);
            var networkProvider = new MockNetworkProvider(BitConverter.GetBytes(6857996259202924925));
            var generator = new NewIdGenerator(tickProvider, networkProvider);

            for (var i = 0; i < 267; i++)
                generator.NextSequentialGuid();

            var guid = generator.NextSequentialGuid();

            Assert.That(guid, Is.EqualTo(expected));
        }

        [Test]
        public void Should_match_sequentially()
        {
            var generator = new NewIdGenerator(_tickProvider, _workerIdProvider);

            var id1 = generator.Next().ToGuid();
            var id2 = generator.NextGuid();
            var id3 = generator.NextGuid();

            Assert.Multiple(() =>
            {
                Assert.That(id2, Is.Not.EqualTo(id1));
                Assert.That(id3, Is.Not.EqualTo(id2));
            });
            Assert.That(id2, Is.GreaterThan(id1));

            Console.WriteLine(id1);
            Console.WriteLine(id2);
            Console.WriteLine(id3);

            var nid1 = id1.ToNewId();
            var nid2 = id2.ToNewId();
        }

        [Test]
        public void Should_match_sequentially_with_sequential_guid()
        {
            var generator = new NewIdGenerator(_tickProvider, _workerIdProvider);

            var nid = generator.Next();
            var id1 = nid.ToSequentialGuid();
            var id2 = generator.NextSequentialGuid();
            var id3 = generator.NextSequentialGuid();

            Assert.Multiple(() =>
            {
                Assert.That(id2, Is.Not.EqualTo(id1));
                Assert.That(id3, Is.Not.EqualTo(id2));
            });
            Assert.That(id2, Is.GreaterThan(id1));

            Console.WriteLine(id1);
            Console.WriteLine(id2);
            Console.WriteLine(id3);

            var nid1 = id1.ToNewIdFromSequential();
            var nid2 = id2.ToNewIdFromSequential();

            Assert.That(nid1, Is.EqualTo(nid));
        }

        [Test]
        public void Should_match_when_all_providers_equal()
        {
            // Arrange
            var generator1 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);
            var generator2 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);

            // Act
            var id1 = generator1.Next();
            var id2 = generator2.Next();

            // Assert
            Assert.That(id2, Is.EqualTo(id1));
        }

        [Test]
        public void Should_match_when_all_providers_equal_with_guid_method()
        {
            // Arrange
            var generator1 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);
            var generator2 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);
            generator1.Next().ToGuid();
            generator2.NextGuid();

            // Act
            var id1 = generator1.Next().ToGuid();
            var id2 = generator2.NextGuid();

            // Assert
            Assert.That(id2, Is.EqualTo(id1));
        }

        [Test]
        public void Should_not_match_when_generated_from_two_processes()
        {
            // Arrange
            var generator1 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);

            _processIdProvider = new MockProcessIdProvider(BitConverter.GetBytes(11));
            var generator2 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);

            // Act
            var id1 = generator1.Next();
            var id2 = generator2.Next();

            // Assert
            Assert.That(id2, Is.Not.EqualTo(id1));
        }

        [Test]
        public void Should_not_match_when_processor_id_provided()
        {
            // Arrange
            var generator1 = new NewIdGenerator(_tickProvider, _workerIdProvider);
            var generator2 = new NewIdGenerator(_tickProvider, _workerIdProvider, _processIdProvider);

            // Act
            var id1 = generator1.Next();
            var id2 = generator2.Next();

            // Assert
            Assert.That(id2, Is.Not.EqualTo(id1));
        }

        [SetUp]
        public void Init()
        {
            _start = DateTime.UtcNow;
            _stopwatch = Stopwatch.StartNew();

            _tickProvider = new MockTickProvider(GetTicks());
            _workerIdProvider = new MockNetworkProvider(BitConverter.GetBytes(1234567890L));
            _processIdProvider = new MockProcessIdProvider(BitConverter.GetBytes(10));
        }

        ITickProvider _tickProvider;
        IWorkerIdProvider _workerIdProvider;
        IProcessIdProvider _processIdProvider;
        DateTime _start;
        Stopwatch _stopwatch;

        long GetTicks()
        {
            return _start.AddTicks(_stopwatch.Elapsed.Ticks).Ticks;
        }


        class MockTickProvider :
            ITickProvider
        {
            public MockTickProvider(long ticks)
            {
                Ticks = ticks;
            }

            public long Ticks { get; }
        }


        class MockNetworkProvider :
            IWorkerIdProvider
        {
            readonly byte[] _workerId;

            public MockNetworkProvider(byte[] workerId)
            {
                _workerId = workerId;
            }

            public byte[] GetWorkerId(int index)
            {
                return _workerId;
            }
        }


        class MockProcessIdProvider :
            IProcessIdProvider
        {
            readonly byte[] _processId;

            public MockProcessIdProvider(byte[] processId)
            {
                _processId = processId;
            }

            public byte[] GetProcessId()
            {
                return _processId;
            }
        }
    }
}
