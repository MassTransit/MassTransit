namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Configuration;
    using Internals;


    public class AssemblyScanner :
        IAssemblyScanner
    {
        readonly List<Assembly> _assemblies = new List<Assembly>();
        readonly CompositeFilter<string> _assemblyFilter = new CompositeFilter<string>();
        readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();

        public int Count => _assemblies.Count;

        public string Description { get; set; }

        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
                _assemblies.Add(assembly);
        }

        public void Assembly(string assemblyName)
        {
            var asm = System.Reflection.Assembly.Load(assemblyName);
            Assembly(asm);
        }

        public void AssemblyContainingType<T>()
        {
            AssemblyContainingType(typeof(T));
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.GetTypeInfo().Assembly);
        }

        public void Exclude(Func<Type, bool> exclude)
        {
            _filter.Excludes += exclude;
        }

        public void ExcludeNamespace(string nameSpace)
        {
            Exclude(type => type.IsInNamespace(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            ExcludeNamespace(typeof(T).Namespace);
        }

        public void Include(Func<Type, bool> predicate)
        {
            _filter.Includes += predicate;
        }

        public void IncludeNamespace(string nameSpace)
        {
            Include(type => type.IsInNamespace(nameSpace));
        }

        public void IncludeNamespaceContainingType<T>()
        {
            IncludeNamespace(typeof(T).Namespace);
        }

        public void ExcludeType<T>()
        {
            Exclude(type => type == typeof(T));
        }

        public void AssembliesFromApplicationBaseDirectory()
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(OnAssemblyLoadFailure, false, _assemblyFilter.Matches);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public void AssembliesAndExecutablesFromPath(string path)
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(path, OnAssemblyLoadFailure, true, _assemblyFilter.Matches);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public void AssembliesFromPath(string path)
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(path, OnAssemblyLoadFailure, false, _assemblyFilter.Matches);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public void AssembliesAndExecutablesFromPath(string path, Func<Assembly, bool> assemblyFilter)
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(path, OnAssemblyLoadFailure, true, _assemblyFilter.Matches)
                .Where(assemblyFilter);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public void AssembliesFromPath(string path, Func<Assembly, bool> assemblyFilter)
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(path, OnAssemblyLoadFailure, false, _assemblyFilter.Matches)
                .Where(assemblyFilter);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public void ExcludeFileNameStartsWith(params string[] startsWith)
        {
            for (var i = 0; i < startsWith.Length; i++)
            {
                var value = startsWith[i];

                _assemblyFilter.Excludes += name => name.StartsWith(value, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void IncludeFileNameStartsWith(params string[] startsWith)
        {
            for (var i = 0; i < startsWith.Length; i++)
            {
                var value = startsWith[i];

                _assemblyFilter.Includes += name => name.StartsWith(value, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void AssembliesAndExecutablesFromApplicationBaseDirectory()
        {
            IEnumerable<Assembly> assemblies = AssemblyFinder.FindAssemblies(OnAssemblyLoadFailure, true, _assemblyFilter.Matches);

            foreach (var assembly in assemblies)
                Assembly(assembly);
        }

        public Task<TypeSet> ScanForTypes()
        {
            return AssemblyTypeCache.FindTypes(_assemblies, _filter.Matches);
        }

        public void TheCallingAssembly()
        {
            var callingAssembly = FindTheCallingAssembly();

            if (callingAssembly != null)
                Assembly(callingAssembly);
            else
                throw new ConfigurationException("Could not determine the calling assembly, you may need to explicitly call IAssemblyScanner.Assembly()");
        }

        static void OnAssemblyLoadFailure(string assemblyName, Exception exception)
        {
            Console.WriteLine("MassTransit could not load assembly from " + assemblyName);
        }

        public bool Contains(string assemblyName)
        {
            return _assemblies
                .Select(assembly => new AssemblyName(assembly.FullName))
                .Any(aName => aName.Name == assemblyName);
        }

        public bool HasAssemblies()
        {
            return _assemblies.Any();
        }

        static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var mtAssembly = typeof(IBus).GetTypeInfo().Assembly;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var declaringType = frame.GetMethod().DeclaringType;
                if (declaringType != null)
                {
                    var assembly = declaringType.GetTypeInfo().Assembly;
                    if (assembly != thisAssembly && assembly != mtAssembly)
                    {
                        callingAssembly = assembly;
                        break;
                    }
                }
            }

            return callingAssembly;
        }
    }
}
