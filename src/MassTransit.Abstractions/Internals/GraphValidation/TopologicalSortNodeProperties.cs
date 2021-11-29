namespace MassTransit.Internals.GraphValidation
{
    public interface ITopologicalSortNodeProperties
    {
        bool Visited { get; set; }
    }
}
