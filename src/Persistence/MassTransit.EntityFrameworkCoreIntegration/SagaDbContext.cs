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

    using MassTransit.Saga;

    using Microsoft.EntityFrameworkCore;

    public class SagaDbContext<TSaga, TEntityConfiguration>:
        DbContext
        where TSaga : class, ISaga
        where TEntityConfiguration : IEntityTypeConfiguration<TSaga>, new()
    {
        public SagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureDefaultSagaMap<TSaga>();

            var configuration = Activator.CreateInstance<TEntityConfiguration>();
            configuration.Configure(modelBuilder.Entity<TSaga>());
        }
    }
}