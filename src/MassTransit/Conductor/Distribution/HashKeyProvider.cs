namespace MassTransit.Conductor.Distribution
{
    public delegate byte[] HashKeyProvider<in T>(T value);
}
