namespace MassTransit.Infrastructure.Saga
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using Magnum.Data;
	using Magnum.Infrastructure.Data;
	using MassTransit.Saga;

	public class NHibernateSagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		public void Dispose()
		{
		}

		public IEnumerator<T> InitiateNewSaga(Guid id)
		{
			try
			{
				using (IUnitOfWork work = UnitOfWork.Start())
				using (ITransaction transaction = work.BeginTransaction(IsolationLevel.Serializable))
				{
					using (var repository = new NHibernateRepository())
					{
						var saga = (T) Activator.CreateInstance(typeof (T), new object[] {id});
						repository.Save(saga);
						work.Flush();

						yield return saga;

						repository.Save(saga);
						transaction.Commit();
					}
				}
			}
			finally
			{
				UnitOfWork.Finish();
			}
		}

		public IEnumerator<T> OrchestrateExistingSaga(Guid id)
		{
			try
			{
				using (IUnitOfWork work = UnitOfWork.Start())
				using (ITransaction transaction = work.BeginTransaction(IsolationLevel.Serializable))
				{
					using (var repository = new NHibernateRepository())
					{
						var saga = repository.Get<T>(id);

						yield return saga;

						repository.Update(saga);
						transaction.Commit();
					}
				}
			}
			finally
			{
				UnitOfWork.Finish();
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			try
			{
				using (IUnitOfWork work = UnitOfWork.Start())
				{
					using (var repository = new NHibernateRepository())
					{
						return repository.List<T>().GetEnumerator();
					}
				}
			}
			finally
			{
				UnitOfWork.Finish();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}