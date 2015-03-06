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
namespace MassTransit.Saga
{
    using System;


    public class SagaContextImpl<TSaga, TMessage> :
        SagaContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Guid _id;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public SagaContextImpl(Guid id, ISagaPolicy<TSaga, TMessage> policy)
        {
            _id = id;
            _policy = policy;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public ISagaPolicy<TSaga, TMessage> Policy
        {
            get { return _policy; }
        }
    }
}