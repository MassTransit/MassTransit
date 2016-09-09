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


    public class AssemblyTypeList
    {
        public readonly IList<Type> Abstract = new List<Type>();
        public readonly IList<Type> Concrete = new List<Type>();
        public readonly IList<Type> Interface = new List<Type>();

        public IEnumerable<IList<Type>> SelectTypes(TypeClassification classification)
        {
            var interfaces = classification.HasFlag(TypeClassification.Interface);
            var concretes = classification.HasFlag(TypeClassification.Concrete);
            var abstracts = classification.HasFlag(TypeClassification.Abstract);

            if (interfaces || concretes || abstracts)
            {
                if (interfaces)
                    yield return Interface;
                if (abstracts)
                    yield return Abstract;
                if (concretes)
                    yield return Concrete;
            }
            else
            {
                yield return Interface;
                yield return Abstract;
                yield return Concrete;
            }
        }

        public IEnumerable<Type> AllTypes()
        {
            return Interface.Concat(Concrete).Concat(Abstract);
        }

        public void Add(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                Interface.Add(type);
            }
            else if (type.GetTypeInfo().IsAbstract)
            {
                Abstract.Add(type);
            }
            else if (type.GetTypeInfo().IsClass)
            {
                Concrete.Add(type);
            }
        }
    }
}