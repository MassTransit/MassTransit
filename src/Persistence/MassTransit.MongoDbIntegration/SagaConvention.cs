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
namespace MassTransit.MongoDbIntegration
{
    using System.Reflection;
    using MassTransit.Saga;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;


    public class SagaConvention :
        ConventionBase,
        IClassMapConvention
    {
        public void Apply(BsonClassMap classMap)
        {
            if (classMap.ClassType.GetTypeInfo().IsClass && typeof(ISaga).IsAssignableFrom(classMap.ClassType))
            {
                classMap.MapIdProperty(nameof(ISaga.CorrelationId));
            }
        }
    }
}