// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Host
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Hosting;
    using Internals.Extensions;
    using Logging;
    using Util;


    public class AssemblyScanner
    {
        readonly string _endpointSpecificationTypeName;
        readonly string _hostBusFactoryTypeName;
        readonly ILog _log = Logger.Get<AssemblyScanner>();
        readonly string _serviceSpecificationTypeName;

        public AssemblyScanner()
        {
            _endpointSpecificationTypeName = TypeMetadataCache<IEndpointSpecification>.ShortName;
            _serviceSpecificationTypeName = TypeMetadataCache<IServiceSpecification>.ShortName;
            _hostBusFactoryTypeName = TypeMetadataCache<IHostBusFactory>.ShortName;
        }

        public IEnumerable<AssemblyRegistration> GetAssemblyRegistrations()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (_log.IsDebugEnabled)
                    _log.Debug($"Scanning assembly directory: {baseDirectory}");

                List<string> assemblies = Directory.EnumerateFiles(baseDirectory, "*.dll", SearchOption.AllDirectories)
                    .Select(x => new {Path = x, File = Path.GetFileName(x)})
                    .Where(x => !x.File.StartsWith("MassTransit.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("Topshelf.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("NewId.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("Newtonsoft.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("log4net.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("NLog.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("Autofac.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("RabbitMQ.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.File.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Path)
                    .ToList();

                var registrations = assemblies
                    .Select(ReflectionOnlyLoadAssembly)
                    .Where(x => x != null)
                    .SelectMany(TrySelectAllTypes)
                    .Where(x => IsSupportedType(x.Item2))
                    .GroupBy(x => x.Item1)
                    .Select(x => new { Assembly = Assembly.Load(x.Key.GetName()), FoundTypes = x })
                    .Select(x => new AssemblyRegistration(x.Assembly, x.FoundTypes.Select(y => x.Assembly.GetType(y.Item2.FullName)).ToArray()))
                    .ToList();

                return registrations;
            }
            finally
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
            }
        }

        public Type GetHostBusFactoryType()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                List<string> assemblies = Directory.EnumerateFiles(baseDirectory, "*.dll", SearchOption.AllDirectories)
                    .Select(x => new {Path = x, File = Path.GetFileName(x)})
                    .Where(x => x.File.StartsWith("MassTransit.", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Path)
                    .ToList();

                var busFactoryType = assemblies
                    .Select(Assembly.ReflectionOnlyLoadFrom)
                    .SelectMany(TrySelectAllTypes)
                    .Where(x => IsHostBusFactoryType(x.Item2))
                    .Select(x => new { Assembly = Assembly.Load(x.Item1.GetName()), x.Item2 })
                    .Select(x => x.Assembly.GetType(x.Item2.FullName))
                    .FirstOrDefault();

                return busFactoryType;
            }
            finally
            {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
            }
        }

        Assembly ReflectionOnlyLoadAssembly(string assemblyFile)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyFile);
            }
            catch (BadImageFormatException e)
            {
                _log.Debug("Could not scan contents of assembly " + assemblyFile, e);
                return null;
            }
        }

        bool IsSupportedType(Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var name = interfaceType.GetTypeName();

                if (name.Equals(_endpointSpecificationTypeName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                if (name.Equals(_serviceSpecificationTypeName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        bool IsHostBusFactoryType(Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var name = interfaceType.GetTypeName();

                if (name.Equals(_hostBusFactoryTypeName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        IEnumerable<Tuple<Assembly, Type>> TrySelectAllTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().Select(type => Tuple.Create(assembly, type));
            }
            catch (ReflectionTypeLoadException e)
            {
                _log.Debug($"Exception loading types from assembly: {assembly.FullName}\n{string.Join(Environment.NewLine, e.LoaderExceptions.Select(x => x.Message))}", e);
                return Enumerable.Empty<Tuple<Assembly, Type>>();
            }
        }
    }
}