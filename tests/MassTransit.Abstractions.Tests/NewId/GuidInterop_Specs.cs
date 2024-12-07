namespace MassTransit.Abstractions.Tests
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_interoperating_with_the_guid_type
    {
        [Test]
        public void Should_convert_from_a_guid_quickly()
        {
            var g = Guid.NewGuid();

            var n = g.ToNewId();

            var ns = n.ToString();
            var gs = g.ToString();

            Assert.That(gs, Is.EqualTo(ns));
        }

        [Test]
        public void Should_convert_to_guid_quickly()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var g = n.ToGuid();

            var ns = n.ToString();
            var gs = g.ToString();

            Assert.That(gs, Is.EqualTo(ns));
        }

        [Test]
        public void Should_convert_to_guid_quickly_from_guid()
        {
            var g = NewId.NextGuid(2)[1]; // ensure sequence number is not 0x0000

            var ns = g.ToNewId().ToString();
            var gs = g.ToString();

            Assert.That(gs, Is.EqualTo(ns));
        }

        [Test]
        public void Should_display_sequentially_for_newid()
        {
            var id = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            Console.WriteLine(id.ToString("DS"));
        }

        [Test]
        public void Should_make_the_round_trip_successfully_via_bytes()
        {
            var g = Guid.NewGuid();

            var n = new NewId(g.ToByteArray());

            var gn = new Guid(n.ToByteArray());

            Assert.That(gn, Is.EqualTo(g));
        }

        [Test]
        public void Should_make_the_round_trip_successfully_via_guid()
        {
            var g = Guid.NewGuid();

            var n = g.ToNewId();

            var gn = n.ToGuid();

            Assert.That(gn, Is.EqualTo(g));
        }

        [Test]
        public void Should_make_the_round_trip_successfully_via_sequential_guid()
        {
            var g = Guid.NewGuid();

            var n = g.ToNewIdFromSequential();

            var gn = n.ToSequentialGuid();

            Assert.That(gn, Is.EqualTo(g));
        }

        [Test]
        public void Should_match_parsed_guid_bs()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var n = new NewId(bytes);

            var ns = n.ToString("Bs");

            var gsn = Guid.Parse(ns);
            var g = n.ToSequentialGuid();

            Assert.That(gsn, Is.EqualTo(g));
        }

        [Test]
        public void Should_match_parsed_guid_ds()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var n = new NewId(bytes);

            var ns = n.ToString("Ds");

            var gsn = Guid.Parse(ns);
            var g = n.ToSequentialGuid();

            Assert.Multiple(() =>
            {
                Assert.That(gsn, Is.EqualTo(g));
                Assert.That(n.ToGuid(), Is.Not.EqualTo(gsn));
            });
        }

        [Test]
        public void Should_match_parsed_guid_ns()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var n = new NewId(bytes);

            var ns = n.ToString("Ns");

            var gsn = Guid.Parse(ns);
            var g = n.ToSequentialGuid();

            Assert.That(gsn, Is.EqualTo(g));
        }

        [Test]
        public void Should_match_parsed_guid_ps()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var n = new NewId(bytes);

            var ns = n.ToString("Ps");

            var gsn = Guid.Parse(ns);
            var g = n.ToSequentialGuid();

            Assert.That(gsn, Is.EqualTo(g));
        }

        [Test]
        public void Should_match_string_output_b()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("B");
            var ns = n.ToString("B");

            Assert.That(ns, Is.EqualTo(gs));
        }

        [Test]
        public void Should_match_string_output_d()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("d");
            var ns = n.ToString("d");

            Assert.That(ns, Is.EqualTo(gs));
        }

        [Test]
        public void Should_match_string_output_n()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("N");
            var ns = n.ToString("N");

            Assert.That(ns, Is.EqualTo(gs));
        }

        [Test]
        public void Should_match_string_output_p()
        {
            var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("P");
            var ns = n.ToString("P");

            Assert.That(ns, Is.EqualTo(gs));
        }

        [Test]
        public void Should_parse_newid_guid_as_newid()
        {
            var n = NewId.Next(2)[1];
            var g = n.ToGuid();

            var ng = NewId.FromGuid(g);

            Assert.That(ng, Is.EqualTo(n));

            // Also checks to see if this would throw
            Assert.That(ng.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void Should_parse_sequential_guid_as_newid()
        {
            var n = NewId.Next(2)[1];

            var nn = n.ToGuid();
            var g = n.ToSequentialGuid();

            var ng = NewId.FromSequentialGuid(g);

            Assert.That(ng, Is.EqualTo(n));

            // Also checks to see if this would throw
            Assert.That(ng.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void Should_properly_handle_string_passthrough()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var ns = n.ToString("D");

            var g = new Guid(ns);

            var nn = new NewId(g.ToString("D"));

            Assert.That(nn, Is.EqualTo(n));
        }

        [Test]
        public void Should_support_the_same_constructor()
        {
            // ensure all bytes have a different value
            var guid = new Guid(0x01020304, 0x0506, 0x0708, 9, 10, 11, 12, 13, 14, 15, 16);
            var newid = new NewId(0x01020304, 0x0506, 0x0708, 9, 10, 11, 12, 13, 14, 15, 16);

            Assert.That(newid.ToString(), Is.EqualTo(guid.ToString()));
        }

        [Test]
        public void Should_work_from_newid_to_guid_to_newid()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var g = new Guid(n.ToByteArray());

            var ng = new NewId(g.ToByteArray());

            Console.WriteLine(g.ToString("D"));

            Assert.That(ng, Is.EqualTo(n));
        }
    }
}
