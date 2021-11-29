namespace MassTransit
{
    using System;


    [Serializable]
    public class JobNotFoundException :
        MassTransitException
    {
        public JobNotFoundException()
        {
        }

        public JobNotFoundException(string message)
            : base(message)
        {
        }
    }
}
