namespace MassTransit.Conductor.Distribution
{
    public delegate string HashKeyProvider<in T>(T value);
}