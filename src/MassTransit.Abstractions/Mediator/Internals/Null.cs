namespace MassTransit.Mediator.Internals
{
    /// <summary>
    /// Represents a null object for a given reference type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Null<T> where T : class
    {
        /// <summary>
        /// The singleton instance representing a null object of type <typeparamref name="T"/>.
        /// </summary>
        public static readonly Null<T> Value = new();

        private Null() { }

        /// <inheritdoc/>
        public override string ToString() => "null";

        public static explicit operator T(Null<T> _) => null!;
    }
}
