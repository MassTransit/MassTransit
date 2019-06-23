namespace MassTransit
{
    using System.Diagnostics;


    public readonly struct StartedActivity
    {
        public readonly DiagnosticSource Source;
        public readonly Activity Activity;

        public StartedActivity(DiagnosticSource source, Activity activity)
        {
            Source = source;
            Activity = activity;
        }

        public void Stop(object args = default)
        {
            Source.StopActivity(Activity, args);
        }
    }
}
