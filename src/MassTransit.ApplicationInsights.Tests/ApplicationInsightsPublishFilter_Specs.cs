// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.ApplicationInsights.Tests
{
    using System;
    using GreenPipes;
    using Microsoft.ApplicationInsights;
    using Moq;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.ApplicationInsights.DataContracts;


    [TestFixture]
    public class ApplicationInsightsPublishFilter_Specs
    {
        Mock<SendHeaders> _mockHeaders;

        [SetUp]
        public void SetUp()
        {
            _mockHeaders = new Mock<SendHeaders>();
        }

        [Test]
        public async Task Should_send_context_to_next_pipe()
        {
            // Arrange.
            var mockPublishContext = new Mock<PublishContext>();
            mockPublishContext.Setup(c => c.Headers).Returns(_mockHeaders.Object);
            var filter = new ApplicationInsightsPublishFilter<PublishContext>(new TelemetryClient(), "", "", null);

            var mockPipe = new Mock<IPipe<PublishContext>>();

            // Act.
            await filter.Send(mockPublishContext.Object, mockPipe.Object);

            // Assert.
            mockPipe.Verify(action => action.Send(mockPublishContext.Object), Times.Once);
        }

        [Test]
        public async Task Should_invoke_the_configure_operation()
        {
            // Arrange.
            var mockPublishContext = new Mock<PublishContext>();
            mockPublishContext.Setup(c => c.Headers).Returns(_mockHeaders.Object);
            bool configureOperationHasBeenCalled = false;

            var filter = new ApplicationInsightsPublishFilter<PublishContext>(new TelemetryClient(), "", "", (holder, context) => configureOperationHasBeenCalled = true);

            // Act.
            await filter.Send(mockPublishContext.Object, new Mock<IPipe<PublishContext>>().Object);

            // Assert.
            Assert.IsTrue(configureOperationHasBeenCalled);
        }

        [Test]
        public async Task Should_add_the_expected_properties_to_the_dependency_telemetry()
        {
            // Arrange.
            var messageId = Guid.NewGuid();
            var conversationId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var requestId = Guid.NewGuid();

            var mockSendContext = new Mock<SendContext>();
            mockSendContext.Setup(c => c.Headers).Returns(_mockHeaders.Object);
            mockSendContext.Setup(c => c.MessageId).Returns(messageId);
            mockSendContext.Setup(c => c.ConversationId).Returns(conversationId);
            mockSendContext.Setup(c => c.CorrelationId).Returns(correlationId);
            mockSendContext.Setup(c => c.RequestId).Returns(requestId);

            var publishContextProxy = new PublishContextProxy<SampleMessage>(mockSendContext.Object, new SampleMessage());

            var capturedTelemetry = default(DependencyTelemetry);

            var filter = new ApplicationInsightsPublishFilter<PublishContext>(new TelemetryClient(), "", "", (holder, context) => capturedTelemetry = holder.Telemetry);

            // Act.
            await filter.Send(publishContextProxy, new Mock<IPipe<PublishContext>>().Object);

            // Assert.
            Assert.IsNotNull(capturedTelemetry);
            Assert.AreEqual(capturedTelemetry.Properties["MessageId"], messageId.ToString());
            Assert.AreEqual(capturedTelemetry.Properties["MessageType"], typeof(SampleMessage).FullName);
            Assert.AreEqual(capturedTelemetry.Properties["ConversationId"], conversationId.ToString());
            Assert.AreEqual(capturedTelemetry.Properties["CorrelationId"], correlationId.ToString());
            Assert.AreEqual(capturedTelemetry.Properties["RequestId"], requestId.ToString());
        }
    }

    public class SampleMessage
    {
    }
}
