namespace MassTransit.Metadata
{
    using System;
    using System.Diagnostics;
    using System.IO;
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
            MachineName = Environment.MachineName;
            MassTransitVersion = typeof(HostInfo).GetTypeInfo().Assembly.GetName().Version.ToString();

            try
            {
                using var currentProcess = Process.GetCurrentProcess();
                ProcessId = currentProcess.Id;
                ProcessName = currentProcess.ProcessName;
                if ("dotnet".Equals(ProcessName, StringComparison.OrdinalIgnoreCase))
                    ProcessName = GetUsefulProcessName(ProcessName);
            }
            catch (PlatformNotSupportedException)
            {
                ProcessId = 0;
                ProcessName = GetUsefulProcessName("UWP");
            }

            var assemblyName = entryAssembly.GetName();
            Assembly = assemblyName.Name;
            AssemblyVersion = assemblyName.Version.ToString();
        }

        public string? MachineName { get; set; }
        public string? ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string? Assembly { get; set; }
        public string? AssemblyVersion { get; set; }
        public string? FrameworkVersion { get; set; }
        public string? MassTransitVersion { get; set; }
        public string? OperatingSystemVersion { get; set; }

        static string GetAssemblyFileVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (attribute != null)
                return attribute.Version;

            var assemblyLocation = assembly.Location;
            if (assemblyLocation != null)
                return FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;

            return "Unknown";
        }

        static string GetAssemblyInformationalVersion(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
                return attribute.InformationalVersion;

            return GetAssemblyFileVersion(assembly);
        }

        static string GetUsefulProcessName(string defaultProcessName)
        {
            var entryAssemblyLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location;

            return string.IsNullOrWhiteSpace(entryAssemblyLocation)
                ? defaultProcessName
                : Path.GetFileNameWithoutExtension(entryAssemblyLocation);
        }
    }
}
