namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using log4net;

	public class InMemorySagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (InMemorySagaRepository<T>));

		private readonly Dictionary<Guid, T> _sagas = new Dictionary<Guid, T>();

		public InMemorySagaRepository()
		{
			_log.Info("Creating new saga repository");
		}

		public void Dispose()
		{
			_sagas.Clear();
		}

		public IEnumerator<T> InitiateNewSaga(Guid id)
		{
			if (_sagas.ContainsKey(id))
				throw new SagaException("The saga already exists and cannot be initiated", typeof (T), typeof (T), id);

			_log.DebugFormat("Creating [{0}] with id {1}", typeof (T).Name, id);

			T saga = (T) Activator.CreateInstance(typeof (T), new object[] {id});

			_sagas.Add(id, saga);

			yield return saga;
		}

		public IEnumerator<T> OrchestrateExistingSaga(Guid id)
		{
			if (!_sagas.ContainsKey(id))
				throw new SagaException("The saga was not found and cannot be loaded", typeof (T), typeof (T), id);

			yield return _sagas[id];
		}
	}
}