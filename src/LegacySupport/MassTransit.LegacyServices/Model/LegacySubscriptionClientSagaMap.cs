// Copyright 2007-2010 The Apache Software Foundation.
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
namespace LegacyRuntime.Model
{
    using FluentNHibernate.Mapping;
    using MassTransit.Infrastructure;
    using MassTransit.LegacySupport;

    public sealed class LegacySubscriptionClientSagaMap :
        ClassMap<LegacySubscriptionClientSaga>
    {
        public LegacySubscriptionClientSagaMap()
        {
            Not.LazyLoad();

            Id(s => s.CorrelationId)
                .GeneratedBy.Assigned();

            Map(x => x.CurrentState)
                .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .CustomType<StateMachineUserType>();

            Map(s => s.ControlUri).CustomType<UriUserType>();
            Map(s => s.DataUri).CustomType<UriUserType>();
        }
    }
}