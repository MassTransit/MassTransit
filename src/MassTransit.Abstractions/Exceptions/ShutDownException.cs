namespace MassTransit
{
    using System;


    [Serializable]
    public class ShutDownException :
        MassTransitException
    {
        public ShutDownException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
