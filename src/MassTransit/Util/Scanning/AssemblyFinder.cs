namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Context;
    using Logging;


    public class AssemblyFinder
    {
        public delegate bool AssemblyFilter(string filename);


        public delegate void AssemblyLoadFailure(string assemblyName, Exception exception);


        public static IEnumerable<Assembly> FindAssemblies(AssemblyLoadFailure loadFailure, bool includeExeFiles, AssemblyFilter filter)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = string.Empty;

            if (string.IsNullOrEmpty(binPath))
                return FindAssemblies(assemblyPath, loadFailure, includeExeFiles, filter);

            if (Path.IsPathRooted(binPath))
                return FindAssemblies(binPath, loadFailure, includeExeFiles, filter);

            string[] binPaths = binPath.Split(';');
            return binPaths.SelectMany(bin =>
            {
                var path = Path.Combine(assemblyPath, bin);
                return FindAssemblies(path, loadFailure, includeExeFiles, filter);
            });
        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, AssemblyLoadFailure loadFailure, bool includeExeFiles, AssemblyFilter filter)
        {
            LogContext.Debug?.Log("Scanning assembly directory: {Path}", assemblyPath);

            IEnumerable<string> dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories).ToList();
            IEnumerable<string> files = dllFiles;

            if (includeExeFiles)
            {
                IEnumerable<string> exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories).ToList();
                files = dllFiles.Concat(exeFiles);
            }

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var filterName = Path.GetFileName(file);
                if (!filter(filterName))
                {
                    LogContext.Debug?.Log("Filtered assembly: {File}", file);

                    continue;
                }

                Assembly assembly = null;
                try
                {
                    assembly = Assembly.ReflectionOnlyLoad(name);
                }
                catch (BadImageFormatException exception)
                {
                    LogContext.Warning?.Log(exception, "Assembly Scan failed: {Name}", name);

                    continue;
                }

                catch (Exception originalException)
                {
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoad(file);
                    }
                    catch (Exception)
                    {
                        loadFailure(file, originalException);
                    }
                }

                if (assembly != null)
                {
                    Assembly loadedAssembly = null;
                    try
                    {
                        loadedAssembly = Assembly.Load(name);
                    }
                    catch (BadImageFormatException exception)
                    {
                        LogContext.Warning?.Log(exception, "Assembly Scan failed: {Name}", name);

                        continue;
                    }
                    catch (Exception originalException)
                    {
                        try
                        {
                            loadedAssembly = Assembly.Load(file);
                        }
                        catch (Exception)
                        {
                            loadFailure(file, originalException);
                        }
                    }

                    if (loadedAssembly != null)
                        yield return loadedAssembly;
                }
            }
        }
    }
}
