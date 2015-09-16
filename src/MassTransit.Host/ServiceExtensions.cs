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
    using System.Text.RegularExpressions;


    public static class ServiceExtensions
    {
        static readonly Regex _regex = new Regex(@"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))");

        public static string GetServiceDescription(this Type bootstrapperType)
        {
            string name = GetDisplayName(bootstrapperType);

            return _regex.Replace(name, " $1");
        }

        /// <summary>
        /// Returns a displayable name of a service, without the suffixes of commonly used types
        /// </summary>
        /// <param name="bootstrapperType"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Type bootstrapperType)
        {
            string name = bootstrapperType.Name;
            if (name.EndsWith("Bootstrapper", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Bootstrapper".Length);
            if (name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Service".Length);
            if (name.EndsWith("BusInstance", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "BusInstance".Length);
            if (name.EndsWith("Specification", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Specification".Length);
            return name;
        }
    }
}