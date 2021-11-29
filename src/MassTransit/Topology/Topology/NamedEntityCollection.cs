namespace MassTransit.Topology
{
    using System;
    using System.Collections.Generic;


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
                    return existingEntity;

                throw new ArgumentException($"The {TypeCache<TEntity>.ShortName} entity settings did not match the existing entity");
            }

            EntityIds.Add(entity.Id, entity);
            Entities.Add(entity, entity);
            _entityNames.Add(entity, entity);

            return entity;
        }
    }
}
