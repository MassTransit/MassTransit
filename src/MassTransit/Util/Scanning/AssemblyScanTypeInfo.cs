namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;


    public class AssemblyScanTypeInfo
    {
        public readonly AssemblyTypeList ClosedTypes = new AssemblyTypeList();
        public readonly AssemblyTypeList OpenTypes = new AssemblyTypeList();

        public AssemblyScanTypeInfo(Assembly assembly)
            : this(assembly.FullName, assembly.GetExportedTypes)
        {
        }

        public AssemblyScanTypeInfo(string name, Func<IEnumerable<Type>> source)
        {
            Record.Name = name;

            try
            {
                IEnumerable<Type> types = source();
                foreach (var type in types)
                {
                    var list = type.IsOpenGeneric() ? OpenTypes : ClosedTypes;
                    list.Add(type);
                }
            }
            catch (Exception ex)
            {
                Record.LoadException = ex;
            }
        }

        public AssemblyScanRecord Record { get; } = new AssemblyScanRecord();

        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            if (classification == TypeClassification.All)
                return ClosedTypes.AllTypes().Concat(OpenTypes.AllTypes());

            if (classification == TypeClassification.Interface)
                return SelectTypes(ClosedTypes.Interface, OpenTypes.Interface);

            if (classification == TypeClassification.Abstract)
                return SelectTypes(ClosedTypes.Abstract, OpenTypes.Abstract);

            if (classification == TypeClassification.Concrete)
                return SelectTypes(ClosedTypes.Concrete, OpenTypes.Concrete);

            if (classification == TypeClassification.Open)
                return OpenTypes.AllTypes();

            if (classification == TypeClassification.Closed)
                return ClosedTypes.AllTypes();

            return SelectTypes(SelectGroups(classification).ToArray());
        }

        IEnumerable<Type> SelectTypes(params IList<Type>[] lists)
        {
            return lists.SelectMany(x => x);
        }

        IEnumerable<IList<Type>> SelectGroups(TypeClassification classification)
        {
            return SelectLists(classification).SelectMany(x => x.SelectTypes(classification));
        }

        IEnumerable<AssemblyTypeList> SelectLists(TypeClassification classification)
        {
            var open = classification.HasFlag(TypeClassification.Open);
            var closed = classification.HasFlag(TypeClassification.Closed);

            if (open && closed || !open && !closed)
            {
                yield return OpenTypes;
                yield return ClosedTypes;
            }
            else if (open)
                yield return OpenTypes;
            else if (closed)
                yield return ClosedTypes;
        }
    }
}
