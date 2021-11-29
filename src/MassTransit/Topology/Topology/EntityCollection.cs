namespace MassTransit.Topology
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public class EntityCollection<TEntity, THandle> :
        IEnumerable<TEntity>
        where TEntity : THandle
        where THandle : EntityHandle
    {
        public EntityCollection(IEqualityComparer<TEntity> entityComparer)
        {
            EntityIds = new Dictionary<long, TEntity>();
            Entities = new Dictionary<TEntity, TEntity>(entityComparer);
        }

        protected IDictionary<TEntity, TEntity> Entities { get; }

        protected IDictionary<long, TEntity> EntityIds { get; }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual THandle GetOrAdd(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // if it's exactly the same exchange
            if (Entities.TryGetValue(entity, out var existingEntity))
                return existingEntity;

            EntityIds.Add(entity.Id, entity);
            Entities.Add(entity, entity);

            return entity;
        }

        public virtual TEntity Get(THandle entityHandle)
        {
            if (entityHandle == null)
                throw new ArgumentNullException(nameof(entityHandle));

            if (!EntityIds.TryGetValue(entityHandle.Id, out var existingEntity))
                throw new ArgumentException($"The existing entity was not found: {TypeCache<TEntity>.ShortName}");

            if (!existingEntity.Equals(entityHandle))
                throw new ArgumentException($"The existing entity did not match the argument entity: {TypeCache<TEntity>.ShortName}");

            return existingEntity;
        }
    }
}
