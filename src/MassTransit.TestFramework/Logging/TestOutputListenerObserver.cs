namespace MassTransit.TestFramework.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Internals;
    using NUnit.Framework;


    class TestOutputListenerObserver :
        IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key.EndsWith(".Start", StringComparison.OrdinalIgnoreCase))
            {
                var activity = Activity.Current;

                TestContext.WriteLine($"{DateTime.Now:HH:mm:ss.fff}-A Start {activity.OperationName} {activity.Id} {activity.ParentId}");
            }

            if (value.Key.EndsWith(".Stop", StringComparison.OrdinalIgnoreCase))
            {
                var activity = Activity.Current;

                TestContext.WriteLine(
                    $"{DateTime.Now:HH:mm:ss.fff}-A Stop  {activity.OperationName} {activity.Id} {activity.ParentId ?? "--"} {activity.Duration.ToFriendlyString()}");
            }
        }
    }
}
