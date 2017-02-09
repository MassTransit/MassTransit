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
namespace MassTransit.EntityFrameworkIntegration
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Reflection;


    public class AssemblyScanningSagaDbContext : DbContext
    {
        readonly Assembly _mappingAssembly;

        public AssemblyScanningSagaDbContext(Assembly mappingAssembly, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            _mappingAssembly = mappingAssembly;
        }

        public AssemblyScanningSagaDbContext(Assembly mappingAssembly, ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            _mappingAssembly = mappingAssembly;
        }

        public AssemblyScanningSagaDbContext(Assembly mappingAssembly, DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            _mappingAssembly = mappingAssembly;
        }

        public AssemblyScanningSagaDbContext(Assembly mappingAssembly, string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            _mappingAssembly = mappingAssembly;
        }

        public AssemblyScanningSagaDbContext(Assembly mappingAssembly, DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            _mappingAssembly = mappingAssembly;
        }

        protected AssemblyScanningSagaDbContext(Assembly mappingAssembly)
        {
            _mappingAssembly = mappingAssembly;
        }

        protected AssemblyScanningSagaDbContext(Assembly mappingAssembly, DbCompiledModel model)
            : base(model)
        {
            _mappingAssembly = mappingAssembly;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) =>
            modelBuilder.Configurations.AddFromAssembly(_mappingAssembly);
    }
}