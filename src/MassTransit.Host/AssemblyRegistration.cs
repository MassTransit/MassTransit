// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Hosting;
    using Internals.Extensions;


    public struct AssemblyRegistration
    {
        public readonly Assembly Assembly;
        public readonly Type[] Types;

        public readonly Type ServiceSpecificationType;
        public readonly Type[] EndpointSpecificationTypes;

        public AssemblyRegistration(Assembly assembly, Type[] types)
        {
            Assembly = assembly;
            Types = types;
            ServiceSpecificationType = GetServiceSpecification(types);
            EndpointSpecificationTypes = GetEndpointSpecifications(types).ToArray();
        }

        static Type GetServiceSpecification(Type[] types)
        {
            return types
                .Where(x => x.HasInterface<IServiceSpecification>())
                .DefaultIfEmpty(typeof(DefaultServiceSpecification))
                .FirstOrDefault();
        }

        static IEnumerable<Type> GetEndpointSpecifications(Type[] types)
        {
            return types.Where(x => x.HasInterface<IEndpointSpecification>());
        }
    }
}