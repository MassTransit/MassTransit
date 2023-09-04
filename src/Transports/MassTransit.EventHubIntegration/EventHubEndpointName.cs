namespace MassTransit
{
    using Configuration;


    public class EventHubEndpointName : IEndpointName
    {
        public string Name { get; private set; }

        public EventHubEndpointName(string eventHubName, string consumerGroup)
        {
            Name = $"{EventHubEndpointAddress.PathPrefix}/{eventHubName}";
            if (!string.IsNullOrWhiteSpace(consumerGroup))
                Name = $"{Name}/{consumerGroup}";
        }
    }
}
