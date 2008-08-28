// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Saga.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using NHibernate;

    public class NHibernateRepository : 
        IRepository
    {
        private readonly ISession _session;

        public NHibernateRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.OpenSession();
        }

        public T Get<T>(object id) where T : class
        {
            return _session.Get<T>(id);
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public T FindBy<T>(Expression<Func<T, bool>> where) where T : class
        {
            // return _session.Linq<T>().Where(where).FirstOrDefault();
            throw new NotSupportedException();
        }

        public IList<T> List<T>() where T : class
        {
            return _session.CreateCriteria(typeof (T)).List<T>();
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> where) where T : class
        {
            //return _session.Linq<T>().Where(where);
            throw new NotSupportedException();
        }

        public void Save<T>(T item) where T : class
        {
            WithinTransaction(() => _session.SaveOrUpdate(item));
        }

        public void Delete<T>(T item) where T : class
        {
            WithinTransaction(() => _session.Delete(item));
        }

        private void WithinTransaction(Action action)
        {
            ITransaction transaction = _session.BeginTransaction();
            try
            {
                action();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}