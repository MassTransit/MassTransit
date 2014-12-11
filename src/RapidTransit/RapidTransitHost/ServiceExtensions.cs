namespace RapidTransit
{
    using System;
    using System.Text.RegularExpressions;


    public static class ServiceExtensions
    {
        static readonly Regex _regex = new Regex(@"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))");

        public static string GetServiceDescription(this Type bootstrapperType)
        {
            string name = GetDisplayName(bootstrapperType);

            return _regex.Replace(name, " $1");
        }

        /// <summary>
        /// Returns a displayable name of a service, without the suffixes of commonly used types
        /// </summary>
        /// <param name="bootstrapperType"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Type bootstrapperType)
        {
            string name = bootstrapperType.Name;
            if (name.EndsWith("Bootstrapper", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Bootstrapper".Length);
            if (name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Service".Length);
            if (name.EndsWith("BusInstance", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "BusInstance".Length);
            return name;
        }
    }
}