namespace nu.Utility
{
    public interface IArgumentMapFactory
    {
        /// <summary>
        /// Creates an argument map for an object using reflection to identify the target properties
        /// </summary>
        /// <param name="obj">An object of the class to map</param>
        /// <returns>A reusable <c ref="ArgumentMap" /> object</returns>
        IArgumentMap CreateMap(object obj);
    }
}