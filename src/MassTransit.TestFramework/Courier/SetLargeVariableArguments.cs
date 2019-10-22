namespace MassTransit.TestFramework.Courier
{
    public interface SetLargeVariableArguments
    {
        string Key { get; }
        MessageData<string> Value { get; }
    }
}
