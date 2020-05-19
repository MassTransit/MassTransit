// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using System;
    using System.Text.RegularExpressions;
    using MassTransit.Topology;


    public class ServiceBusEntityNameValidator :
        IEntityNameValidator
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:\/\$]+$", RegexOptions.Compiled);

        public void ThrowIfInvalidEntityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The entity name must not be null or empty");

            var success = IsValidEntityName(name);
            if (!success)
            {
                throw new ArgumentException("The entity name must be a sequence of these characters: letters, digits, hyphen, underscore, period, slash, or colon.");
            }
        }

        public bool IsValidEntityName(string name)
        {
            return _regex.Match(name).Success;
        }

        public static IEntityNameValidator Validator => Cached.EntityNameValidator;


        static class Cached
        {
            internal static readonly IEntityNameValidator EntityNameValidator = new ServiceBusEntityNameValidator();
        }
    }
}
