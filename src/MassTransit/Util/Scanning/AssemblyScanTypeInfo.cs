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
namespace MassTransit.Util.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals.Extensions;


    public class AssemblyScanTypeInfo
    {
        readonly AssemblyScanRecord _record = new AssemblyScanRecord();

        public readonly AssemblyTypeList ClosedTypes = new AssemblyTypeList();
        public readonly AssemblyTypeList OpenTypes = new AssemblyTypeList();

        public AssemblyScanTypeInfo(Assembly assembly)
            : this(assembly.FullName, assembly.GetExportedTypes)
        {
        }

        public AssemblyScanTypeInfo(string name, Func<IEnumerable<Type>> source)
        {
            _record.Name = name;

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
                _record.LoadException = ex;
            }
        }

        public AssemblyScanRecord Record => _record;

        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            if (classification == TypeClassification.All)
            {
                return ClosedTypes.AllTypes().Concat(OpenTypes.AllTypes());
            }

            if (classification == TypeClassification.Interface)
            {
                return SelectTypes(ClosedTypes.Interface, OpenTypes.Interface);
            }

            if (classification == TypeClassification.Abstract)
            {
                return SelectTypes(ClosedTypes.Abstract, OpenTypes.Abstract);
            }

            if (classification == TypeClassification.Concrete)
            {
                return SelectTypes(ClosedTypes.Concrete, OpenTypes.Concrete);
            }

            if (classification == TypeClassification.Open)
            {
                return OpenTypes.AllTypes();
            }

            if (classification == TypeClassification.Closed)
            {
                return ClosedTypes.AllTypes();
            }

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

            if ((open && closed) || (!open && !closed))
            {
                yield return OpenTypes;
                yield return ClosedTypes;
            }
            else if (open)
            {
                yield return OpenTypes;
            }
            else if (closed)
            {
                yield return ClosedTypes;
            }
        }
    }
}