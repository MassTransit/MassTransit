namespace MassTransit.Transports.InMemory.GraphValidation
{
    interface ITopologicalSortNodeProperties
    {
        bool Visited { get; set; }
    }
}