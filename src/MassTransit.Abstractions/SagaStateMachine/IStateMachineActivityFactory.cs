namespace MassTransit
{
    public interface IStateMachineActivityFactory
    {
        /// <summary>
        /// Returns the service, if available, otherwise returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>(PipeContext context)
            where T : class;
    }
}
