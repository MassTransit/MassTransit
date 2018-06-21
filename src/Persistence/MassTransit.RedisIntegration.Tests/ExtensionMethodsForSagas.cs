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

namespace MassTransit.RedisIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using RedisIntegration;
    using Saga;


    public static class ExtensionMethodsForSagas
    {
        public static async Task<bool> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId, TimeSpan timeout)
            where TSaga : class, ISaga
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                var saga = await (repository as IRetrieveSagaFromRepository<TSaga>).GetSaga(sagaId);
                if (saga != null)
                    return true;
                await Task.Delay(10);
            }

            return false;
        }

        public static async Task<bool> ShouldContainSaga<TSaga>(this ISagaRepository<TSaga> repository, Guid sagaId, Func<TSaga, bool> condition,
            TimeSpan timeout)
            where TSaga : class, ISaga
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                var saga = await (repository as IRetrieveSagaFromRepository<TSaga>).GetSaga(sagaId);
                if (condition(saga))
                    return true;
                await Task.Delay(10);
            }

            return false;
        }
    }
}