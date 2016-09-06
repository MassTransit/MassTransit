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