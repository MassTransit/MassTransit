namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Net;

    public static class MsmqUriParser
    {
        /// <summary>
		/// The name of the queue in local format (.\private$\name)
		/// </summary>
        public static string GetLocalName(Uri uri)
        {
            return @".\private$\" + uri.AbsolutePath.Substring(1);
        }

        /// <summary>
		/// The format name used to talk to MSMQ
		/// </summary>
        public static string GetFormatName(Uri uri)
        {
            string hostName = uri.Host;

			if (IsIpAddress(hostName))
				return string.Format(@"FormatName:DIRECT=TCP:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));

			return string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, uri.AbsolutePath.Substring(1));
        }

        private static bool IsIpAddress(string hostName)
        {
            IPAddress address;
            return IPAddress.TryParse(hostName, out address);
        }
    }
}