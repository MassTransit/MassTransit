namespace MassTransit.Metadata.Domain
{
    using System;


    public class TransmissionMetadata
    {
        public Uri From { get; set; }
        public Uri To { get; set; }
        public string Message { get; set; }
        public DateTime SentOn { get; set; }
    }
}