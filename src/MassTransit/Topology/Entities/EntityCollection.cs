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
    using System.Collections;
    using System.Collections.Generic;
    using Metadata;
    using Util;


    public class EntityCollection<TEntity, THandle> :
        IEnumerable<TEntity>
        where TEntity : THandle
        where THandle : EntityHandle
    {
        readonly IDictionary<TEntity, TEntity> _entities;
        readonly IDictionary<long, TEntity> _entityIds;

        public EntityCollection(IEqualityComparer<TEntity> entityComparer)
        {
            _entityIds = new Dictionary<long, TEntity>();
            _entities = new Dictionary<TEntity, TEntity>(entityComparer);
        }

        protected IDictionary<TEntity, TEntity> Entities => _entities;
        protected IDictionary<long, TEntity> EntityIds => _entityIds;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual THandle GetOrAdd(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            TEntity existingEntity;
            // if it's exactly the same exchange
            if (_entities.TryGetValue(entity, out existingEntity))
            {
                return existingEntity;
            }

            _entityIds.Add(entity.Id, entity);
            _entities.Add(entity, entity);

            return entity;
        }

        public virtual TEntity Get(THandle entityHandle)
        {
            if (entityHandle == null)
                throw new ArgumentNullException(nameof(entityHandle));

            TEntity existingEntity;
            if (!_entityIds.TryGetValue(entityHandle.Id, out existingEntity))
                throw new ArgumentException($"The existing entity was not found: {TypeMetadataCache<TEntity>.ShortName}");

            if (!existingEntity.Equals(entityHandle))
                throw new ArgumentException($"The existing entity did not match the argument entity: {TypeMetadataCache<TEntity>.ShortName}");

            return existingEntity;
        }
    }
}