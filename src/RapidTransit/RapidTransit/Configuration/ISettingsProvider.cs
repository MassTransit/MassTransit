namespace RapidTransit.Configuration
{
    public interface ISettingsProvider
    {
        /// <summary>
        /// Try to get the settings of the specified type, using the specified prefix
        /// </summary>
        /// <typeparam name="T">The settings type to return</typeparam>
        /// <param name="prefix">The prefix to use when resolving configuration values from the property names</param>
        /// <param name="settings">The output settings value</param>
        /// <returns>True if the settings could be resolved</returns>
        bool TryGetSettings<T>(string prefix, out T settings)
            where T : ISettings;

        /// <summary>
        /// Try to get the settings of the specified type, using the specified prefix
        /// </summary>
        /// <typeparam name="T">The settings type to return</typeparam>
        /// <param name="settings">The output settings value</param>
        /// <returns>True if the settings could be resolved</returns>
        bool TryGetSettings<T>(out T settings)
            where T : ISettings;
    }
}