namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Scanning;


    /// <summary>
    /// Access to a set of exported .Net Type's as defined in a scanning operation
    /// </summary>
    public class TypeSet
    {
        readonly IEnumerable<AssemblyScanTypeInfo> _allTypes;
        readonly Func<Type, bool> _filter;

        public TypeSet(IEnumerable<AssemblyScanTypeInfo> allTypes, Func<Type, bool> filter = null)
        {
            _allTypes = allTypes;
            _filter = filter;
        }

        /// <summary>
        /// For diagnostic purposes, explains which assemblies were
        /// scanned as part of this TypeSet, including failures
        /// </summary>
        public IEnumerable<AssemblyScanRecord> Records
        {
            get { return _allTypes.Select(x => x.Record); }
        }

        /// <summary>
        /// Find any types in this TypeSet that match any combination of the TypeClassification enumeration values
        /// </summary>
        /// <param name="classification"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            IEnumerable<Type> types = _allTypes.SelectMany(x => x.FindTypes(classification));

            return _filter == null ? types : types.Where(_filter);
        }

        /// <summary>
        /// Returns all the types in this TypeSet
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> AllTypes()
        {
            IEnumerable<Type> types = _allTypes.SelectMany(x => x.FindTypes(TypeClassification.All));

            return _filter == null ? types : types.Where(_filter);
        }
    }
}
