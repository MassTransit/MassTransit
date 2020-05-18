// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public sealed class AmazonSqsHostEqualityComparer :
        IEqualityComparer<AmazonSqsHostSettings>
    {
        public static IEqualityComparer<AmazonSqsHostSettings> Default { get; } = new AmazonSqsHostEqualityComparer();

        public bool Equals(AmazonSqsHostSettings x, AmazonSqsHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;

            return string.Equals(x.Region.SystemName, y.Region.SystemName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(AmazonSqsHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.AccessKey?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (obj.Region?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
