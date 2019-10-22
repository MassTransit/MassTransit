// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Reflection;
    using Context;
    using Hosting;
    using Internals.Extensions;
    using Metadata;
    using Util;
    using Util.Scanning;


    public class ServiceAssemblyScanner
    {
        readonly string _endpointSpecificationTypeName;
        readonly string _hostBusFactoryTypeName;
        readonly string _serviceSpecificationTypeName;

        public ServiceAssemblyScanner()
        {
            _endpointSpecificationTypeName = TypeMetadataCache<IEndpointSpecification>.ShortName;
            _serviceSpecificationTypeName = TypeMetadataCache<IServiceSpecification>.ShortName;
            _hostBusFactoryTypeName = TypeMetadataCache<IHostBusFactory>.ShortName;
        }

        public IEnumerable<AssemblyRegistration> GetAssemblyRegistrations()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

            var scanner = new AssemblyScanner();
            scanner.ExcludeFileNameStartsWith("Topshelf.", "NewId.", "Newtonsoft.", "log4net.", "NLog.", "Autofac.", "RabbitMQ.", "Microsoft.", "System.",
                "SQLite.");
            scanner.Include(IsSupportedType);

            try
            {
                scanner.AssembliesFromApplicationBaseDirectory();

                var typeSet = scanner.ScanForTypes().Result;

                List<AssemblyRegistration> registrations = typeSet.FindTypes(TypeClassification.Concrete | TypeClassification.Closed)
                    .GroupBy(x => x.Assembly)
                    .Select(x => new AssemblyRegistration(x.Key, x.ToArray()))
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
                var scanner = new AssemblyScanner();
                scanner.IncludeFileNameStartsWith("MassTransit.");
                scanner.Include(IsHostBusFactoryType);

                scanner.AssembliesFromApplicationBaseDirectory();

                var typeSet = scanner.ScanForTypes().Result;

                var busFactoryType = typeSet.FindTypes(TypeClassification.Concrete | TypeClassification.Closed)
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
                LogContext.Warning?.Log(e, "Assembly Scan failed: {File}", assemblyFile);
                return null;
            }
        }

        bool IsSupportedType(Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var name = TypeMetadataCache.GetShortName(interfaceType);

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
                var name = TypeMetadataCache.GetShortName(interfaceType);

                if (name.Equals(_hostBusFactoryTypeName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}
