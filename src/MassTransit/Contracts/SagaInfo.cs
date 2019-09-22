namespace MassTransit.Contracts
{
    /// <summary>
    /// Describes a saga
    /// </summary>
    public interface SagaInfo
    {
        string SagaType { get; }

        /// <summary>
        /// Messages which can initiate a new saga instance
        /// </summary>
        MessageInfo[] Initiating { get; }

        /// <summary>
        /// Messages which are orchestrated by an existing saga instance
        /// </summary>
        MessageInfo[] Orchestrating { get; }

        /// <summary>
        /// Messages which are observed by an existing saga instance
        /// </summary>
        MessageInfo[] Observing { get; }
    }
}
