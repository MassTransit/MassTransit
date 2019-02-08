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
namespace MassTransit.Definition
{
    using System;
    using System.Linq.Expressions;
    using System.Text;


    public class ConsumerMessageDefinitionConfigurator<TConsumer, TMessage> :
        IConsumerMessageDefinitionConfigurator<TConsumer, TMessage>
        where TConsumer : class, IConsumer<TMessage>
        where TMessage : class
    {
        public void Publishes<T>()
            where T : class
        {
            throw new NotImplementedException();
        }

        public void Sends<T>()
            where T : class
        {
            throw new NotImplementedException();
        }

        public void Facet<T>(Expression<Func<TMessage, T>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>> configure = null)
        {
            throw new NotImplementedException();
        }

        public void Property<T>(Expression<Func<TMessage, T>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, T>> configure = null)
        {
            throw new NotImplementedException();
        }

        public void PartitionBy(Expression<Func<TMessage, Guid>> propertyExpression)
        {
            throw new NotImplementedException();
        }

        public void PartitionBy(Expression<Func<TMessage, string>> propertyExpression, Encoding encoding = default)
        {
            throw new NotImplementedException();
        }

        public void Resource(Expression<Func<TMessage, Uri>> propertyExpression,
            Action<IConsumerMessagePropertyDefinitionConfigurator<TConsumer, TMessage, Uri>> configure = null)
        {
            throw new NotImplementedException();
        }
    }
}
