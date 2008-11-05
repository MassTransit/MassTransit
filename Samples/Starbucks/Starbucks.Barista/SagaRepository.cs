using System;
using System.Collections.Generic;
using MassTransit.Saga;

namespace Starbucks.Barista
{
    public class DrinkPreparationSagaRepository : ISagaRepository<DrinkPreparationSaga>
    {
        //this could go in a database
        private IDictionary<Guid, DrinkPreparationSaga> _sagas = new Dictionary<Guid, DrinkPreparationSaga>();        

        public void Dispose()
        {            
        }

        public DrinkPreparationSaga Create(Guid correlationId)
        {
            var saga = new DrinkPreparationSaga(correlationId);
            lock(_sagas)
            {
                _sagas[correlationId] = saga;
            }

            return saga;
        }

        public DrinkPreparationSaga Get(Guid id)
        {
            lock (_sagas)
            {
                return _sagas[id];
            }
        }

        public void Save(DrinkPreparationSaga item)
        {
            //this is where I'd update a database
        }
    }
}