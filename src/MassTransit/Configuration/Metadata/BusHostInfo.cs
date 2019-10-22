namespace MassTransit.Metadata
{
    using System;
    using System.Diagnostics;
    using System.Reflection;


    [Serializable]
    public class BusHostInfo :
        HostInfo
    {
        public BusHostInfo()
        {
        }

        public BusHostInfo(bool initialize)
        {
            FrameworkVersion = Environment.Version.ToString();
            OperatingSystemVersion = Environment.OSVersion.ToString();
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetCallingAssembly();
            var currentProcess = Process.GetCurrentProcess();
            MachineName = Environment.MachineName;
            MassTransitVersion = typeof(IBus).GetTypeInfo().Assembly.GetName().Version.ToString();
            ProcessId = currentProcess.Id;
            ProcessName = currentProcess.ProcessName;

            var assemblyName = entryAssembly.GetName();
            Assembly = assemblyName.Name;
            AssemblyVersion = assemblyName.Version.ToString();
        }

        public string MachineName { get; private set; }
        public string ProcessName { get; private set; }
        public int ProcessId { get; private set; }
        public string Assembly { get; private set; }
        public string AssemblyVersion { get; private set; }
        public string FrameworkVersion { get; private set; }
        public string MassTransitVersion { get; private set; }
        public string OperatingSystemVersion { get; private set; }

        static string GetAssemblyFileVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attribute != null)
            {
                return attribute.Version;
            }

            var assemblyLocation = assembly.Location;
            if (assemblyLocation != null)
                return FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;

            return "Unknown";
        }

        static string GetAssemblyInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
            {
                return attribute.InformationalVersion;
            }

            return GetAssemblyFileVersion(assembly);
        }
    }
}
