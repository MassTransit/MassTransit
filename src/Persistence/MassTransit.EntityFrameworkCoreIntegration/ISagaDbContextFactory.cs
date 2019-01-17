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
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    /// <summary>
    /// Creates a DbContext for the saga repository
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaDbContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a standalone DbContext
        /// </summary>
        /// <returns></returns>
        DbContext Create();

        /// <summary>
        /// Create a scoped DbContext within the lifetime scope of the saga repository
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbContext CreateScoped<T>(ConsumeContext<T> context)
            where T : class;

        /// <summary>
        /// Release the DbContext once it is no longer needed
        /// </summary>
        /// <param name="dbContext"></param>
        void Release(DbContext dbContext);
    }
}