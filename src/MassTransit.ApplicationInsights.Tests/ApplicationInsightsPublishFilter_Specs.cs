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
	using GreenPipes;
	using Microsoft.ApplicationInsights;
	using Moq;
	using NUnit.Framework;
	using System.Threading.Tasks;

	public class ApplicationInsightsPublishFilter_Specs
	{
		[Test]
		public async Task Should_send_context_to_next_pipe()
		{
			bool configureOperationHasBeenCalled = false;

			var mockPublishContext = new Mock<PublishContext>();
			mockPublishContext.SetupGet(p => p.Headers).Returns(Mock.Of<SendHeaders>());

			var filter = new ApplicationInsightsPublishFilter<PublishContext>(new TelemetryClient(), "", "", ((holder, context) => configureOperationHasBeenCalled = true));

			var mockPipe = new Mock<IPipe<PublishContext>>();
			await filter.Send(mockPublishContext.Object, mockPipe.Object);

			Assert.IsTrue(configureOperationHasBeenCalled);
			mockPipe.Verify(action => action.Send(mockPublishContext.Object), Times.Once);
		}
	}
}