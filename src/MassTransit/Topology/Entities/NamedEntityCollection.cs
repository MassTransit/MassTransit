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
namespace MassTransit.Topology.Entities
{
    using System;
    using System.Collections.Generic;
    using Metadata;
    using Util;


    public class NamedEntityCollection<TEntity, THandle> :
        EntityCollection<TEntity, THandle>
        where TEntity : THandle
        where THandle : EntityHandle
    {
        readonly IDictionary<TEntity, TEntity> _entityNames;

        public NamedEntityCollection(IEqualityComparer<TEntity> entityComparer, IEqualityComparer<TEntity> nameComparer)
            : base(entityComparer)
        {
            _entityNames = new Dictionary<TEntity, TEntity>(nameComparer);
        }

        public override THandle GetOrAdd(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_entityNames.TryGetValue(entity, out var existingEntity))
            {
                // if it's exactly the same exchange
                if (Entities.TryGetValue(entity, out existingEntity))
                {
                    return existingEntity;
                }

                throw new ArgumentException($"The {TypeMetadataCache<TEntity>.ShortName} entity settings did not match the existing entity");
            }

            EntityIds.Add(entity.Id, entity);
            Entities.Add(entity, entity);
            _entityNames.Add(entity, entity);

            return entity;
        }
    }
}