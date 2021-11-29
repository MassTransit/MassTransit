namespace MassTransit
{
    /// <summary>
    /// Used to visit the state machine structure, so it can be displayed, etc.
    /// </summary>
    public interface IVisitable :
        IProbeSite
    {
        /// <summary>
        /// A visitable site can accept the visitor and pass control to internal elements
        /// </summary>
        /// <param name="visitor"></param>
        void Accept(StateMachineVisitor visitor);
    }
}
