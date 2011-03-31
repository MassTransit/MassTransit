using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit.Transports;
using NUnit.Framework;

namespace MassTransit.Tests
{
    [TestFixture]
    public class MessageRetryTracker_Specs
    {
        [Test]
        public void IsLimitExceeded_return_false_untill_specified_limit_reached()
        {
            const int retryLimit = 5;

            var tracker = new MessageRetryTracker(retryLimit);
            const string id = "qelofjsw";

            for (int i = 0; i < retryLimit; i++)
            {
                Assert.IsFalse(tracker.IsRetryLimitExceeded(id));
                tracker.IncrementRetryCount(id);
            }
            Assert.IsTrue(tracker.IsRetryLimitExceeded(id));
        }
    }
}
