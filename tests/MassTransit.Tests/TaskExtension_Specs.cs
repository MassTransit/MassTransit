namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using MassTransit.Initializers;


    public class TaskExtension_Specs
    {
        [Test]
        public async Task Should_return_the_string_property()
        {
            var subject = Task.FromResult(new Subject {Name = "Frank"});

            var result = await subject.Select(x => x.Name);

            Assert.That(result, Is.EqualTo("Frank"));
        }

        [Test]
        public async Task Should_return_the_default_string_property()
        {
            var subject = Task.FromResult(new Subject());

            var result = await subject.Select(x => x.Name, "Frank");

            Assert.That(result, Is.EqualTo("Frank"));
        }

        [Test]
        public async Task Should_return_the_default_provider_string_property()
        {
            var subject = Task.FromResult(new Subject());

            var result = await subject.Select(x => x.Name, () => "Frank");

            Assert.That(result, Is.EqualTo("Frank"));
        }

        [Test]
        public async Task Should_return_the_default_async_string_property()
        {
            var subject = Task.FromResult(new Subject());

            var result = await subject.Select(x => x.Name, () => Task.FromResult("Frank"));

            Assert.That(result, Is.EqualTo("Frank"));
        }

        [Test]
        public async Task Should_return_the_value_type_property()
        {
            var subject = Task.FromResult(new Subject {Id = 27});

            var result = await subject.Select(x => x.Id);

            Assert.That(result, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_return_the_nullable_value_type_property()
        {
            var subject = Task.FromResult(new Subject {MemberId = 27});

            var result = await subject.Select(x => x.MemberId);

            Assert.That(result, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_return_the_null_nullable_value_type_property()
        {
            var subject = Task.FromResult(new Subject {MemberId = default});

            var result = await subject.Select(x => x.MemberId);

            Assert.That(result.HasValue, Is.False);
        }

        [Test]
        public async Task Should_return_the_null_nullable_default_value_for_value_type_property()
        {
            var subject = Task.FromResult(new Subject {MemberId = default});

            var result = await subject.Select(x => x.MemberId, 27);

            Assert.That(result, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_return_the_null_nullable_default_value_provider_for_value_type_property()
        {
            var subject = Task.FromResult(new Subject {MemberId = default});

            var result = await subject.Select(x => x.MemberId, () => 27);

            Assert.That(result, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_return_the_null_nullable_default_value_async_provider_for_value_type_property()
        {
            var subject = Task.FromResult(new Subject {MemberId = default});

            var result = await subject.Select(x => x.MemberId, () => Task.FromResult(27));

            Assert.That(result, Is.EqualTo(27));
        }


        class Subject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? MemberId { get; set; }
        }
    }
}
