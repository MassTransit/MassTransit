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
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Courier;
    using Courier.Documents;
    using Courier.Events;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;
    using Saga;


    public class MassTransitMongoDbConventions
    {
        public MassTransitMongoDbConventions(ConventionFilter filter = default(ConventionFilter))
        {
            var conventionFilter = filter ?? IsMassTransitClass;

            var convention = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new SagaConvention(),
                new MemberDefaultValueConvention(typeof(Guid), Guid.Empty)
            };

            ConventionRegistry.Register("MassTransitConventions", convention, type => conventionFilter(type));

            RegisterClass<RoutingSlipDocument>(x => x.TrackingNumber);
            RegisterClass<ExceptionInfoDocument>();
            RegisterClass<ActivityExceptionDocument>();
            RegisterClass<HostDocument>();
            RegisterClass<RoutingSlipEventDocument>();
            RegisterClass<RoutingSlipActivityCompensatedDocument>();
            RegisterClass<RoutingSlipActivityCompensationFailedDocument>();
            RegisterClass<RoutingSlipActivityCompletedDocument>();
            RegisterClass<RoutingSlipActivityFaultedDocument>();
            RegisterClass<RoutingSlipCompensationFailedDocument>();
            RegisterClass<RoutingSlipCompletedDocument>();
            RegisterClass<RoutingSlipFaultedDocument>();
            RegisterClass<RoutingSlipRevisedDocument>();
            RegisterClass<RoutingSlipTerminatedDocument>();
        }

        static bool IsMassTransitClass(Type type)
        {
            return type.FullName.StartsWith("MassTransit") || IsSagaClass(type);
        }

        static bool IsSagaClass(Type type)
        {
            return type.GetTypeInfo().IsClass && typeof(IVersionedSaga).IsAssignableFrom(type);
        }

        public void RegisterClass<T>(Expression<Func<T, Guid>> id)
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T)))
                return;

            BsonClassMap.RegisterClassMap<T>(x =>
            {
                x.AutoMap();
                x.SetIdMember(x.GetMemberMap(id));
            });
        }

        public void RegisterClass<T>()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T)))
                return;

            BsonClassMap.RegisterClassMap<T>(x =>
            {
                x.AutoMap();
                x.SetDiscriminatorIsRequired(true);

                var typeName = typeof(T).Name;
                if (typeName.EndsWith("Document"))
                    x.SetDiscriminator(typeName.Substring(0, typeName.Length - "Document".Length));
            });
        }
    }
}