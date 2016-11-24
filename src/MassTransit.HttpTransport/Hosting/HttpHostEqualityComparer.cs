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
namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using System.Collections.Generic;


    public sealed class HttpHostEqualityComparer :
        IEqualityComparer<HttpHostSettings>
    {
        public static IEqualityComparer<HttpHostSettings> Default { get; } = new HttpHostEqualityComparer();

        public bool Equals(HttpHostSettings x, HttpHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            return string.Equals(x.Host, y.Host, StringComparison.OrdinalIgnoreCase)
                && CheckScheme(x.Scheme, y.Scheme)
                && x.Port == y.Port;
        }

        static bool CheckScheme(string x, string y)
        {
            var xr = x == "reply" ? "http" : x == "replys" ? "https" : x;
            var xy = y == "reply" ? "http" : y == "replys" ? "https" : y;
            return string.Equals(xr, xy, StringComparison.OrdinalIgnoreCase);
        }
        public int GetHashCode(HttpHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.Host?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Port;
                return hashCode;
            }
        }
    }
}