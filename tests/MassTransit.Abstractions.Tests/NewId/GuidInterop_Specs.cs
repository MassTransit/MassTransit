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

            Assert.AreEqual(ns, gs);
        }

        [Test]
        public void Should_convert_to_guid_quickly()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var g = n.ToGuid();

            var ns = n.ToString();
            var gs = g.ToString();

            Assert.AreEqual(ns, gs);
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

            Assert.AreEqual(g, gn);
        }

        [Test]
        public void Should_make_the_round_trip_successfully_via_guid()
        {
            var g = Guid.NewGuid();

            var n = g.ToNewId();

            var gn = n.ToGuid();

            Assert.AreEqual(g, gn);
        }

        [Test]
        public void Should_match_string_output_b()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("B");
            var ns = n.ToString("B");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_d()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("d");
            var ns = n.ToString("d");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_n()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("N");
            var ns = n.ToString("N");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_match_string_output_p()
        {
            var bytes = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

            var g = new Guid(bytes);
            var n = new NewId(bytes);

            var gs = g.ToString("P");
            var ns = n.ToString("P");

            Assert.AreEqual(gs, ns);
        }

        [Test]
        public void Should_properly_handle_string_passthrough()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var ns = n.ToString("D");

            var g = new Guid(ns);

            var nn = new NewId(g.ToString("D"));

            Assert.AreEqual(n, nn);
        }

        [Test]
        public void Should_support_the_same_constructor()
        {
            // ensure all bytes have a different value
            var guid = new Guid(0x01020304, 0x0506, 0x0708, 9, 10, 11, 12, 13, 14, 15, 16);
            var newid = new NewId(0x01020304, 0x0506, 0x0708, 9, 10, 11, 12, 13, 14, 15, 16);

            Assert.AreEqual(guid.ToString(), newid.ToString());
        }

        [Test]
        public void Should_work_from_newid_to_guid_to_newid()
        {
            var n = NewId.Next(2)[1]; // ensure sequence number is not 0x0000

            var g = new Guid(n.ToByteArray());

            var ng = new NewId(g.ToByteArray());

            Console.WriteLine(g.ToString("D"));

            Assert.AreEqual(n, ng);
        }


        [Test]
        public void Should_parse_newid_guid_as_newid()
        {
            NewId n = NewId.Next();

            var g = n.ToGuid();

            var ng = NewId.FromGuid(g);

            Assert.AreEqual(n, ng);

            // Also checks to see if this would throw
            Assert.IsTrue(ng.Timestamp != default);
        }

        [Test]
        public void Should_parse_sequential_guid_as_newid()
        {
            NewId n = NewId.Next();

            var nn = n.ToGuid();
            var g = n.ToSequentialGuid();

            var ng = NewId.FromSequentialGuid(g);

            Assert.AreEqual(n, ng);

            // Also checks to see if this would throw
            Assert.IsTrue(ng.Timestamp != default);
        }
    }
}
