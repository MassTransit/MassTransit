namespace MassTransit.Registration
{
    using System;
    using System.Linq;


    public static class UriRegistrationDisplayExtensions
    {
        /// <summary>
        /// Returns the last part of the <see cref="Uri.AbsolutePath"/> trimmed and neat for display
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string GetLastPart(this Uri address)
        {
            return address?.AbsolutePath.Split('/').LastOrDefault();
        }
    }
}
