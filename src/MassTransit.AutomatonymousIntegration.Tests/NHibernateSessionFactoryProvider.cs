// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Cfg.Loquacious;
    using NHibernate.Mapping.ByCode;
    using NHibernate.Tool.hbm2ddl;


    public class NHibernateSessionFactoryProvider
    {
        static readonly Mutex _factoryMutex = new Mutex();
        readonly Configuration _configuration;
        readonly Action<IDbIntegrationConfigurationProperties> _databaseIntegration;
        readonly IEnumerable<Type> _mappedTypes;
        bool _computed;
        ISessionFactory _sessionFactory;

        public NHibernateSessionFactoryProvider(IEnumerable<Type> mappedTypes)
        {
            _mappedTypes = mappedTypes;

            _configuration = CreateConfiguration();
        }

        protected NHibernateSessionFactoryProvider(IEnumerable<Type> mappedTypes,
            Action<IDbIntegrationConfigurationProperties> databaseIntegration)
        {
            _mappedTypes = mappedTypes;
            _databaseIntegration = databaseIntegration;
            _configuration = CreateConfiguration();
        }

        public Configuration Configuration => _configuration;

        /// <summary>
        ///     Builds the session factory and returns the ISessionFactory. If it was already
        ///     built, the same instance is returned.
        /// </summary>
        /// <returns></returns>
        public virtual ISessionFactory GetSessionFactory()
        {
            if (_computed)
                return _sessionFactory;

            return CreateSessionFactory();
        }

        /// <summary>
        ///     Update the schema in the database
        /// </summary>
        public void UpdateSchema()
        {
            new SchemaUpdate(_configuration).Execute(false, true);
        }

        ModelMapper CreateModelMapper()
        {
            var mapper = new ModelMapper();

            mapper.AfterMapProperty += (inspector, member, customizer) =>
            {
                var memberType = member.LocalMember.GetPropertyOrFieldType();

                if (memberType.IsGenericType
                    && typeof(Nullable<>).IsAssignableFrom(memberType.GetGenericTypeDefinition()))
                    customizer.NotNullable(false);
                else if (!typeof(string).IsAssignableFrom(memberType))
                    customizer.NotNullable(true);
            };

            mapper.AddMappings(_mappedTypes);

            return mapper;
        }

        ISessionFactory CreateSessionFactory()
        {
            try
            {
                var acquired = _factoryMutex.WaitOne();
                if (!acquired)
                    throw new InvalidOperationException("Waiting for access to create session factory failed.");

                var sessionFactory = _configuration.BuildSessionFactory();

                _sessionFactory = sessionFactory;
                _computed = true;

                return sessionFactory;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create session factory", ex);
            }
            finally
            {
                _factoryMutex.ReleaseMutex();
            }
        }

        Configuration ApplyDatabaseIntegration(Configuration configuration)
        {
            if (_databaseIntegration == null)
                configuration = configuration.Configure();

            configuration.DataBaseIntegration(c =>
            {
                if (_databaseIntegration != null)
                    _databaseIntegration(c);

                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.SchemaAction = SchemaAutoAction.Update;
            });

            return configuration;
        }

        Configuration CreateConfiguration()
        {
            var mapper = CreateModelMapper();

            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();

            configuration = ApplyDatabaseIntegration(configuration);

            configuration.AddMapping(domainMapping);

            return configuration;
        }
    }
}