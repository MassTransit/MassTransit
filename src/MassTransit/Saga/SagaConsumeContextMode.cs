namespace MassTransit.Saga
{
    public enum SagaConsumeContextMode
    {
        /// <summary>
        /// Existing saga loaded from storage
        /// </summary>
        Load = 0,

        /// <summary>
        /// New saga created
        /// </summary>
        Add = 1,

        /// <summary>
        /// New saga inserted prior to event
        /// </summary>
        Insert = 2
    }
}
