// Copyright 2007-2011 The Apache Software Foundation.
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
namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Transports;
    using NUnit.Framework;

    [TestFixture]
    public class The_message_retry_tracker
    {
        [Test]
        public void Should_return_false_until_the_limit_is_exceeded()
        {
            const int retryLimit = 5;

            var tracker = new InMemoryInboundMessageTracker(retryLimit);
            const string id = "qelofjsw";

            Exception ex;
            IEnumerable<Action> fa;

            for (int i = 0; i < retryLimit; i++)
            {
                Assert.IsFalse(tracker.IsRetryLimitExceeded(id, out ex, out fa));
                tracker.IncrementRetryCount(id, ex);
            }
            Assert.IsTrue(tracker.IsRetryLimitExceeded(id, out ex, out fa));
        }

        [Test]
        public void Should_reset_once_the_message_was_received()
        {
            const int retryLimit = 5;

            var tracker = new InMemoryInboundMessageTracker(retryLimit);
            const string id = "qelofjsw";

            Exception ex;
            IEnumerable<Action> fa;
            Assert.IsFalse(tracker.IsRetryLimitExceeded(id, out ex, out fa));
            tracker.IncrementRetryCount(id, ex);

            tracker.MessageWasReceivedSuccessfully(id);

            for (int i = 0; i < retryLimit; i++)
            {
                Assert.IsFalse(tracker.IsRetryLimitExceeded(id, out ex, out fa));
                tracker.IncrementRetryCount(id, ex);
            }
            Assert.IsTrue(tracker.IsRetryLimitExceeded(id, out ex, out fa));
        }
    }
}