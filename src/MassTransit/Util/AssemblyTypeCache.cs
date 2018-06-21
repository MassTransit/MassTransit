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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Scanning;


    /// <summary>
    /// Caches assemblies and assembly types to avoid repeated assembly scanning
    /// </summary>
    public static class AssemblyTypeCache
    {
        /// <summary>
        /// Remove all cached assemblies, essentially forcing a reload of any new assembly scans
        /// </summary>
        public static void Clear()
        {
            Cached.Assemblies.Clear();
        }

        /// <summary>
        /// Use to assert that there were no failures in type scanning when trying to find the exported types
        /// from any Assembly
        /// </summary>
        public static void ThrowIfAnyTypeScanFailures()
        {
            IEnumerable<Exception> exceptions = FailedAssemblies().Select(x => x.Record.LoadException).ToList();
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public static IEnumerable<AssemblyScanTypeInfo> FailedAssemblies()
        {
            Task<AssemblyScanTypeInfo>[] tasks = Cached.Assemblies.Select(x => x.Value).ToArray();
            Task.WaitAll(tasks);

            return tasks.Where(x => x.Result.Record.LoadException != null).Select(x => x.Result);
        }

        public static Task<AssemblyScanTypeInfo> ForAssembly(Assembly assembly)
        {
            return Cached.Assemblies.GetOrAdd(assembly, assem => Task.Factory.StartNew(() => new AssemblyScanTypeInfo(assem)));
        }

        public static Task<TypeSet> FindTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> filter = null)
        {
            Task<AssemblyScanTypeInfo>[] tasks = assemblies.Select(ForAssembly).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, assems =>
            {
                return new TypeSet(assems.Select(x => x.Result).ToArray(), filter);
            });
        }

        public static Task<IEnumerable<Type>> FindTypes(IEnumerable<Assembly> assemblies, TypeClassification classification, Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            Task<IEnumerable<Type>>[] tasks = assemblies.Select(assem => ForAssembly(assem).ContinueWith(t => query.Find(t.Result))).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, results => results.SelectMany(x => x.Result));
        }

        public static Task<IEnumerable<Type>> FindTypes(Assembly assembly, TypeClassification classification, Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            return ForAssembly(assembly).ContinueWith(t => query.Find(t.Result));
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Assembly, Task<AssemblyScanTypeInfo>> Assemblies =
                new ConcurrentDictionary<Assembly, Task<AssemblyScanTypeInfo>>();
        }
    }
}