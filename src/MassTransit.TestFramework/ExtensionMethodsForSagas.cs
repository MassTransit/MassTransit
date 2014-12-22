// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Saga;


    public static class ExtensionMethodsForSagas
    {
        public static async Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                Guid saga = (await repository.Where(x => x.CorrelationId == sagaId)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10);
            }

            return default(Guid?);
        }

        public static async Task<Guid?> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository,
            Expression<Func<TSaga, bool>> filter,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            var sagaFilter = new SagaFilter<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await repository.Find(sagaFilter)).ToList();
                if (sagas.Count > 0)
                    return sagas.Single();

                await Task.Delay(10);
            }

            return default(Guid?);
        }
    }
}