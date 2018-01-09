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
namespace MassTransit.Initializers.Conventions
{
    /// <summary>
    /// Looks for a property that can be used as a CorrelationId message header, and
    /// applies a filter to set it on message send if available
    /// </summary>
    public class InitializerConvention :
        IInitializerConvention
    {
        readonly IConventionTypeCache<IMessageTypeInitializerConvention> _typeCache;

        public InitializerConvention(IConventionTypeCacheFactory<IMessageTypeInitializerConvention> cacheFactory)
        {
            _typeCache = new ConventionTypeCache<IMessageTypeInitializerConvention>(cacheFactory);
        }

        public bool TryGetMessagePropertyInitializer<TMessage, TInput, TProperty>(string propertyName,
            out IMessagePropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IMessageInitializerConvention<TMessage>>()
                .TryGetMessagePropertyInitializer<TInput, TProperty>(propertyName, out initializer);
        }
    }
}