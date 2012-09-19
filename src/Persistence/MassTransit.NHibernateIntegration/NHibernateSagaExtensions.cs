// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Magnum.StateMachine;
    using NHibernate.Mapping.ByCode;
    using NHibernateIntegration;

    public static class NHibernateSagaExtensions
    {
        public static void StateProperty<T>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression)
            where T : class
        {
            mapper.Property(stateExpression, x =>
                {
                    x.Access(Accessor.Field);
                    x.Type<StateMachineUserType>();
                    x.NotNullable(true);

                    x.Length(80);
                });
        }

        public static void StateProperty<T>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression, int length)
            where T : class
        {
            mapper.Property(stateExpression, x =>
                {
                    x.Access(Accessor.Field);
                    x.Type<StateMachineUserType>();
                    x.NotNullable(true);

                    x.Length(length);
                });
        }

        public static void StateProperty<T>(this IClassMapper<T> mapper,
            Expression<Func<T, State>> stateExpression, Action<IPropertyMapper> callback)
            where T : class
        {
            mapper.Property(stateExpression, x =>
                {
                    x.Access(Accessor.Field);
                    x.Type<StateMachineUserType>();
                    x.NotNullable(true);

                    callback(x);
                });
        }
    }
}