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
    using Microsoft.ApplicationInsights;
    using Moq;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_Specs
    {
        [Test]
        public void Bus_should_be_created_when_use_ApplicationInsights_extension()
        {
            var telemetryClient = new TelemetryClient();

            void CreateBus() =>
                Bus.Factory.CreateUsingInMemory(x =>
                {
                    x.UseApplicationInsightsOnConsume(telemetryClient, (operation, context) => operation.Telemetry.Properties.Add("prop", "v"));
                });

            Assert.DoesNotThrow(CreateBus);
		}

		[Test]
		public void Bus_should_be_created_when_use_ApplicationInsightsOnSend_extension()
		{
			var telemetryClient = new TelemetryClient();

			void CreateBus() =>
				Bus.Factory.CreateUsingInMemory(x =>
				{
					x.UseApplicationInsightsOnSend(
                        telemetryClient,
                        configureOperation: (holder, context) =>
                        {
                            holder.Telemetry.Properties.Add("key", "value");
                        });
				});

			Assert.DoesNotThrow(CreateBus);
		}

		[Test]
        public void Bus_should_be_created_when_use_ApplicationInsightsOnPublish_extension()
        {
            var telemetryClient = new TelemetryClient();

            void CreateBus() =>
                Bus.Factory.CreateUsingInMemory(x =>
                {
                    x.UseApplicationInsightsOnPublish(
                        telemetryClient,
                        configureOperation: (holder, context) =>
                        {
                            holder.Telemetry.Properties.Add("key", "value");
                        });
                });

            Assert.DoesNotThrow(CreateBus);
        }

        [Test]
        public void Should_call_ConfigurePublish_on_the_given_pipeline_configurator()
        {
            // Arrange.
            var pipelineConfiguratorMock = new Mock<IPublishPipelineConfigurator>();
            var pipelineConfigurator = pipelineConfiguratorMock.Object;
            var telemetryClient = new TelemetryClient();

            // Act.
            pipelineConfigurator.UseApplicationInsightsOnPublish(telemetryClient);

            // Assert.
            pipelineConfiguratorMock.Verify(c => c.ConfigurePublish(It.IsAny<Action<IPublishPipeConfigurator>>()), Times.Once);
        }
    }
}
