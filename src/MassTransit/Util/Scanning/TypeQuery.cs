namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class TypeQuery
    {
        readonly TypeClassification _classification;

        public readonly Func<Type, bool> Filter;

        public TypeQuery(TypeClassification classification, Func<Type, bool> filter = null)
        {
            Filter = filter ?? (t => true);
            _classification = classification;
        }

        public IEnumerable<Type> Find(AssemblyScanTypeInfo assembly)
        {
            return assembly.FindTypes(_classification).Where(Filter);
        }
    }
}
