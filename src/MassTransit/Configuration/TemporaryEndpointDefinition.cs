// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    /// <summary>
    /// Specifies a temporary endpoint, with the prefix "response"
    /// </summary>
    public class TemporaryEndpointDefinition :
        IEndpointDefinition
    {
        readonly string _tag;

        public TemporaryEndpointDefinition(string tag = default, int? concurrentMessageLimit = default, int? prefetchCount = default)
        {
            ConcurrentMessageLimit = concurrentMessageLimit;
            PrefetchCount = prefetchCount;

            _tag = tag ?? "endpoint";
        }

        public string GetEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.TemporaryEndpoint(_tag);
        }

        public bool IsTemporary => true;
        public int? PrefetchCount { get; }
        public int? ConcurrentMessageLimit { get; }

        public void Configure<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
        }
    }
}
