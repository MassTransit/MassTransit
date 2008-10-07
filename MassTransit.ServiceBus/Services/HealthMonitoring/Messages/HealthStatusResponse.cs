namespace MassTransit.ServiceBus.HealthMonitoring.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class HealthStatusResponse :
        CorrelatedBy<Guid>
    {
        private readonly IList<HealthInformation> _healthInformation;
        private readonly Guid _correlationId;

        public HealthStatusResponse(IList<HealthInformation> healthInformation, Guid correlationId)
        {
            _healthInformation = healthInformation;
            _correlationId = correlationId;
        }

        public IList<HealthInformation> HealthInformation
        {
            get { return _healthInformation; }
        }

        #region Implementation of CorrelatedBy<Guid>

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        #endregion
    }
}