namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Diagnostics;


    public static class Utils
    {
        public static TimeSpan Timeout
        {
            get
            {
                if (Debugger.IsAttached)
                    return TimeSpan.FromMinutes(10);

                return TimeSpan.FromSeconds(8);
            }
        }
    }
}
