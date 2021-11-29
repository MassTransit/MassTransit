namespace MassTransit
{
    /// <summary>
    /// For saga repositories that use an incrementing version
    /// </summary>
    public interface ISagaVersion :
        ISaga
    {
        int Version { get; set; }
    }
}
