namespace MassTransit.Tests.Services.Metadata
{
    using System;
    using MassTransit.Services.Metadata;
    using MassTransit.Services.Metadata.Messages;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MessageMetadataExtraction_spec
    {
        [Test]
        public void I_need_to_extract_metadata_from_objects()
        {
            var metadataExtracter = new MetadataExtracter.MetadataExtractor();
            MessageDefinition metadata = metadataExtracter.Extract<PingMessage>();

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
    }
}