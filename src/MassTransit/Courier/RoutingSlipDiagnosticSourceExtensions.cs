namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Contracts;
    using Logging;
    using Metadata;


    public static class RoutingSlipDiagnosticSourceExtensions
    {
        public static StartedActivity? StartExecuteActivity<TActivity, TArguments>(this EnabledDiagnosticSource source, ConsumeContext<RoutingSlip> context)
            where TActivity : IExecuteActivity<TArguments>
            where TArguments : class
        {
            return source.StartActivity(GetBags(context.Message), GetActivityTags<TActivity>(context.Message.TrackingNumber,
                DiagnosticHeaders.ArgumentType, TypeMetadataCache<TArguments>.ShortName));
        }

        public static StartedActivity? StartCompensateActivity<TActivity, TLog>(this EnabledDiagnosticSource source, ConsumeContext<RoutingSlip> context)
            where TActivity : ICompensateActivity<TLog>
            where TLog : class
        {
            return source.StartActivity(GetBags(context.Message), GetActivityTags<TActivity>(context.Message.TrackingNumber,
                DiagnosticHeaders.LogType, TypeMetadataCache<TLog>.ShortName));
        }

        static IEnumerable<(string, string)> GetBags(RoutingSlip routingSlip)
        {
            yield return (DiagnosticHeaders.TrackingNumber, routingSlip.TrackingNumber.ToString("D"));
        }

        static IEnumerable<(string, string)> GetActivityTags<TActivity>(Guid trackingNumber, string key, string value)
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            yield return (DiagnosticHeaders.PeerHost, HostMetadataCache.Host.ProcessName);
            yield return (DiagnosticHeaders.PeerAddress, GetActivityName(typeof(TActivity).Name));
            yield return (DiagnosticHeaders.PeerService, "Activity");

            yield return (DiagnosticHeaders.ActivityType, TypeMetadataCache<TActivity>.ShortName);
            yield return (key, value);
            yield return (DiagnosticHeaders.TrackingNumber, trackingNumber.ToString("D"));
        }

        static string GetActivityName(string typeName)
        {
            const string activity = "Activity";

            var activityName = typeName;
            if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - activity.Length);

            return activityName;
        }
    }
}
