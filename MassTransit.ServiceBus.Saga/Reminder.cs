namespace MassTransit.ServiceBus.Saga
{
    using System;

    /// <summary>
    /// A reminder that will send a <see cref="TimeoutMessage"/> to a saga 
    /// after a period of time.
    /// </summary>
    public class Reminder
    {
        /// <summary>
        /// Sets how long until the specified saga should be sent
        /// a <see cref="TimeoutMessage"/>.
        /// </summary>
        /// <param name="span">The time period to wait.</param>
        /// <param name="saga">The saga to remind.</param>
        /// <param name="state">An object that will be returned to the saga object when the Timespan passes.</param>
        public void ExpireIn(TimeSpan span, ISagaController saga, object state)
        {
            TimeoutMessage msg = new TimeoutMessage();
            msg.Expires = DateTime.Now + span;
            msg.SagaId = saga.Id;

            this.bus.Send(this.timeoutManagerAddress, msg);
        }

        #region config info

        private IServiceBus bus;

        /// <summary>
        /// Gets/sets the bus that will be used to send the <see cref="TimeoutMessage"/>.
        /// </summary>
        public IServiceBus Bus
        {
            get { return bus; }
            set { bus = value; }
        }

        private IEndpoint timeoutManagerAddress;

        /// <summary>
        /// Sets the address of the endpoint where the TimeoutManager process is running.
        /// </summary>
        public IEndpoint TimeoutManagerAddress
        {
            set { timeoutManagerAddress = value; }
        }

        #endregion
    }
}