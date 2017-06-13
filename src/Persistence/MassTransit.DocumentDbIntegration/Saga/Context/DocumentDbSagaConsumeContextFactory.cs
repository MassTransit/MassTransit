// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;

    public class DocumentDbSagaConsumeContextFactory :
        IDocumentDbSagaConsumeContextFactory
    {
        public SagaConsumeContext<TSaga, TMessage> Create<TSaga, TMessage>(IDocumentClient client, string databaseName, string collectionName, ConsumeContext<TMessage> message, TSaga instance,
            bool existing = true)
            where TSaga : class, ISaga
            where TMessage : class
        {
            return new DocumentDbSagaConsumeContext<TSaga, TMessage>(client, databaseName, collectionName, message, instance, existing);
        }
    }
}