namespace MassTransit.Internals.GraphValidation
{
    interface ITopologicalSortNodeProperties
    {
        bool Visited { get; set; }
    }
}