namespace MassTransit.JobService.Scheduling;

readonly struct ValueAndPosition
{
    public ValueAndPosition(int value, int position)
    {
        Value = value;
        Position = position;
    }

    public int Value { get; }
    public int Position { get; }

    public void Deconstruct(out int value, out int position)
    {
        value = Value;
        position = Position;
    }
}
