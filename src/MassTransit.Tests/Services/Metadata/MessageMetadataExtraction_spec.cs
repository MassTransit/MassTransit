namespace MassTransit.Tests.Services.Metadata
{
    using System;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Services.Metadata;
    using MassTransit.Services.Metadata.Messages;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class MessageMetadataExtraction_spec
    {
        [Test]
        public void I_need_to_extract_metadata_from_objects()
        {
            var metadataExtracter = new MetadataExtracter();
            MessageDefinition metadata = metadataExtracter.Extract(typeof(PingMessage));

            metadata.Name
                .ShouldEqual(typeof (PingMessage).FullName);
            metadata.DotNetType
                .ShouldEqual(typeof (PingMessage).FullName);
            metadata.Parent
                .ShouldBeNull();
            metadata.Children
                .ShouldNotBeEmpty();

            metadata.Children[0].Name
                .ShouldEqual("CorrelationId");
            metadata.Children[0].DotNetType
                .ShouldEqual(typeof (Guid).Name);
        }

        [Test]
        public void PipelineBuilding()
        {
            var builder = MockRepository.GenerateMock<IObjectBuilder>();
            MessagePipeline pipeline = MessagePipelineConfigurator.CreateDefault(builder, null);

            pipeline.Subscribe<MetadataConsumer>();
            pipeline.Subscribe<MetadataPConsumer>();

            PipelineViewer.Trace(pipeline);
        }

        public bool ExtractMetadata(object message)
        {
            MetadataExtracter e = new MetadataExtracter();
            e.Extract(message.GetType());
            return true;
        }

        public class MetadataConsumer :
            Consumes<object>.All
        {
            public void Consume(object message)
            {
                throw new System.NotImplementedException();
            }
        }

        public class MetadataPConsumer :
            Consumes<PingMessage>.All
        {
            public void Consume(PingMessage message)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}