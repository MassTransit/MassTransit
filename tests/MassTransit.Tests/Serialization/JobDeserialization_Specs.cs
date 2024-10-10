namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using InMemoryTransport;
    using JobSerialization;
    using MassTransit.Initializers;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;


    namespace JobSerialization
    {
        using System.Collections.Generic;


        public interface VideoDetail
        {
            public string Value { get; set; }
        }


        public interface ConvertVideo
        {
            string GroupId { get; }
            int Index { get; }
            int Count { get; }
            string Path { get; }

            IList<VideoDetail> Details { get; }
        }
    }


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(MessagePackMessageSerializer))]
    public class JobDeserialization_Specs :
        SerializationTest
    {
        [Test]
        public async Task Should_deserialize_the_job()
        {
            ConsumeContext<ConvertVideo> convertVideoContext = await GetConsumeContext<ConvertVideo>(new
            {
                path = "Hello",
                groupId = "group1",
                Index = 0,
                Count = 1,
                Details = new[] { new { Value = "first" }, new { Value = "second" } }
            });

            ConsumeContext<StartJob> startJobContext = await GetConsumeContext<StartJob>(new
            {
                JobId = NewId.NextGuid(),
                AttemptId = NewId.NextGuid(),
                Job = convertVideoContext.ToDictionary(convertVideoContext.Message),
                JobTypeId = NewId.NextGuid()
            });

            var job = startJobContext.GetJob<ConvertVideo>()
                ?? throw new SerializationException($"The job could not be deserialized: {TypeCache<ConvertVideo>.ShortName}");

            Assert.That(job.Details, Has.Count.EqualTo(2));
        }

        protected async Task<ConsumeContext<T>> GetConsumeContext<T>(object values)
            where T : class
        {
            var bytes = Serialize((await MessageInitializerCache<T>.Initialize(values)).Message);

            var message = new InMemoryTransportMessage(NewId.NextGuid(), bytes, Serializer.ContentType.MediaType);
            var receiveContext = new InMemoryReceiveContext(message, TestConsumeContext.GetContext());

            var consumeContext = Deserializer.Deserialize(receiveContext);

            Assert.That(consumeContext.TryGetMessage(out ConsumeContext<T> messageContext), Is.True);

            Assert.That(messageContext, Is.Not.Null);

            return messageContext;
        }

        public JobDeserialization_Specs(Type serializerType)
            : base(serializerType)
        {
        }
    }
}
