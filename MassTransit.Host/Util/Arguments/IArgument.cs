namespace nu.Utility
{
    /// <summary>
    /// Base interface for an individual argument
    /// </summary>
    public interface IArgument
    {
        string Key { get; }

        /// <summary>
        /// The value of the argument
        /// </summary>
        string Value { get; }
    }
}