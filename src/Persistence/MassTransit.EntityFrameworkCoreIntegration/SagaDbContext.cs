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
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals.Extensions;
    using Mappings;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;


    public abstract class SagaDbContext :
        DbContext
    {
        protected abstract IEnumerable<ISagaClassMap> Configurations { get; }

        protected SagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var configuration in Configurations)
                configuration.Configure(modelBuilder);
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
        #if DEBUG
            if (typeof(TEntity).HasInterface<ISaga>() && !ContainsSagaConfiguration<TEntity>())
                throw new ApplicationException($"Context should contains {TypeMetadataCache<TEntity>.ShortName} configuration");
        #endif
            return base.Set<TEntity>();
        }

        bool ContainsSagaConfiguration<T>()
        {
            return Configurations != null && Configurations.Any(x => x.SagaType == typeof(T));
        }
    }
}
