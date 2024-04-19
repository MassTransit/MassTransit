namespace MassTransit.Abstractions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NewIdFormatters;
    using NewIdParsers;
    using NUnit.Framework;


    [TestFixture]
    public class Using_the_newid_formatters
    {
        // Base32
        [Test]
        public void Should_compare_known_conversions_Base32Lower() => CompareKnownEncoding("Base32Lower", new Base32Formatter());

        [Test]
        public void Should_compare_known_conversions_Base32Upper() => CompareKnownEncoding("Base32Upper", new Base32Formatter(true));

        [Test]
        public void Should_compare_known_conversions_CustomBase32()
        {
            CompareKnownEncoding("CustomBase32", new Base32Formatter("0123456789ABCDEFGHIJKLMNOPQRSTUV"));
        }

        [Test]
        public void Should_compare_known_conversions_DashedHexBase16BracketsLower()
        {
            CompareKnownEncoding("DashedHexBase16BracketsLower", new DashedHexFormatter('{', '}'));
        }

        [Test]
        public void Should_compare_known_conversions_DashedHexBase16BracketsUpper()
        {
            CompareKnownEncoding("DashedHexBase16BracketsUpper", new DashedHexFormatter('{', '}', true));
        }

        // DashedHex
        [Test]
        public void Should_compare_known_conversions_DashedHexBase16Lower()
        {
            CompareKnownEncoding("DashedHexBase16Lower", new DashedHexFormatter());
        }

        [Test]
        public void Should_compare_known_conversions_DashedHexBase16Upper()
        {
            CompareKnownEncoding("DashedHexBase16Upper", new DashedHexFormatter(upperCase: true));
        }

        // Hex
        [Test]
        public void Should_compare_known_conversions_HexBase16Lower() => CompareKnownEncoding("HexBase16Lower", new HexFormatter());

        [Test]
        public void Should_compare_known_conversions_HexBase16Upper() => CompareKnownEncoding("HexBase16Upper", new HexFormatter(true));

        // ZBase32
        [Test]
        public void Should_compare_known_conversions_ZBase32Lower()
        {
            CompareKnownEncoding("ZBase32Lower", new ZBase32Formatter());
        }

        [Test]
        public void Should_compare_known_conversions_ZBase32Upper()
        {
            CompareKnownEncoding("ZBase32Upper", new ZBase32Formatter(true));
        }

        [Test]
        public void Should_convert_back_using_parser()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new ZBase32Formatter(true);

            var ns = n.ToString(formatter);

            var parser = new ZBase32Parser();
            var newId = parser.Parse(ns);


            Assert.That(newId, Is.EqualTo(n));
        }

        [Test]
        public void Should_convert_back_using_standard_parser()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter(true);

            var ns = n.ToString(formatter);

            var parser = new Base32Parser();
            var newId = parser.Parse(ns);


            Assert.That(newId, Is.EqualTo(n));
        }

        [Test]
        public void Should_convert_using_custom_base32_formatting_characters()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter("0123456789ABCDEFGHIJKLMNOPQRSTUV");

            var ns = n.ToString(formatter);

            Assert.That(ns, Is.EqualTo("UQP7OV4AN129HB4N79GGF8GJ10"));
        }

        [Test]
        public void Should_convert_using_standard_base32_formatting_characters()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new Base32Formatter(true);

            var ns = n.ToString(formatter);

            Assert.That(ns, Is.EqualTo("62ZHY7EKXBCJRLEXHJQQPIQTBA"));
        }

        [Test]
        public void Should_convert_using_the_optimized_human_readable_formatter()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var formatter = new ZBase32Formatter(true);

            var ns = n.ToString(formatter);

            Assert.That(ns, Is.EqualTo("6438A9RKZBNJTMRZ8JOOXEOUBY"));
        }

        [Test]
        public void Should_translate_often_transposed_characters_to_proper_values()
        {
            var n = new NewId("F6B27C7C-8AB8-4498-AC97-3A6107A21320");

            var ns = "6438A9RK2BNJTMRZ8J0OXE0UBY";

            var parser = new ZBase32Parser(true);
            var newId = parser.Parse(ns);


            Assert.That(newId, Is.EqualTo(n));
        }

        readonly Dictionary<string, string[]> _testValues;

        public Using_the_newid_formatters()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var textsFileName = Path.Combine(directory, "NewId", "texts.txt");
            var fileText = File.ReadAllText(textsFileName);

            _testValues = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(fileText);
        }

        public void CompareKnownEncoding(string name, INewIdFormatter formatter)
        {
            var guids = _testValues["Guids"];
            var expectedValues = _testValues[name];
            Assert.That(expectedValues, Has.Length.EqualTo(guids.Length));

            for (var i = 0; i < guids.Length; i++)
            {
                var newId = new NewId(guids[i]);
                var text = newId.ToString(formatter);
                Assert.That(text, Is.EqualTo(expectedValues[i]));
            }

            Console.WriteLine("Compared {0} equal conversions", guids.Length);
        }
    }
}
