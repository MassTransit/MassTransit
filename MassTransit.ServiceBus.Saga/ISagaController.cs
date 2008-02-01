namespace MassTransit.ServiceBus.Saga
{
    using System;

    /// <summary>
    /// Defines the basic functionality of an MassTransit saga.
    /// </summary>
    public interface ISagaController
    {
        /// <summary>
        /// Gets/sets the Id of the workflow. Do NOT generate this value in your code.
        /// The value of the Id will be generated automatically to provide the
        /// best performance for saving in a database.
        /// </summary>
        /// <remarks>
        /// The reason Guid is used for workflow Id is that messages containing this Id need
        /// to be sent by the workflow even before it is persisted.
        /// </remarks>
        Guid Id { get; set; }

        /// <summary>
        /// Gets whether or not the workflow has completed.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Notifies the workflow that the <see cref="Reminder" />
        /// it previously requested has passed.
        /// </summary>
        /// <param name="state">The object passed as a parameter <see cref="state" />
        /// to the ExpireIn method of <see cref="Reminder"/>.</param>
        void Timeout(object state);
    }
}