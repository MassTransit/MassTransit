namespace MassTransit.ServiceBus.Saga
{
    using System;

    /// <summary>
    /// A message to signal a saga that a reminder was set.
    /// </summary>
    [Serializable]
    public class TimeoutMessage : ISagaMessage
    {
        private DateTime expires;

        /// <summary>
        /// Gets/sets the date and time at which the timeout message is due to expire.
        /// </summary>
        public DateTime Expires
        {
            get { return expires; }
            set { expires = value; }
        }

        private Guid sagaId;

        /// <summary>
        /// Gets/sets the Id of the workflow the TimeoutMessage is connected to.
        /// </summary>
        public Guid SagaId
        {
            get { return sagaId; }
            set { sagaId = value; }
        }

        private object state;

        /// <summary>
        /// Contains the object passed as the state parameter
        /// to the ExpireIn method of <see cref="Reminder"/>.
        /// </summary>
        public object State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets whether or not the TimeoutMessage has expired.
        /// </summary>
        /// <returns>true if the message has expired, otherwise false.</returns>
        public bool HasNotExpired()
        {
            return DateTime.Now < this.expires;
        }
    }
}