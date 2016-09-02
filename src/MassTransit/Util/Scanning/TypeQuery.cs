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


    public class TypeQuery
    {
        private readonly TypeClassification _classification;

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