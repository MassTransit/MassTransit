namespace MassTransit.Metadata.Messages
{
    using System;

    [Serializable]
    public class TransmissionModel
    {
        public Uri From { get; set; }
        public Uri To { get; set; }
        public string Message { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}