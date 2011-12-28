namespace MassTransit.Diagnostics.Introspection
{
    using System;

    public static class DiagnosticsInfo
    {
        public static void WriteCommonItems(DiagnosticsProbe probe)
        {
            probe.Add("mt.version", typeof(IServiceBus).Assembly.GetName().Version);
            probe.Add("host.machine_name", Environment.MachineName);
            OperatingSystem(probe);
            Process(probe); 
        }
        static void Process(DiagnosticsProbe probe)
        {
            var msg = "PID?";
            if (Environment.Is64BitProcess)
                msg = msg + " (x64)";
         
            probe.Add("os.process",msg);
        }

        static void OperatingSystem(DiagnosticsProbe probe)
        {
            var msg = Environment.OSVersion.ToString();
            if (Environment.Is64BitOperatingSystem)
                msg = msg + " (x64)";
            
            probe.Add("os", msg);
        }
    }
}