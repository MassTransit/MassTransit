using System;
using MassTransit.Exceptions;

namespace MassTransit.Configuration
{
    public static class BusConfigurationConvienceExtensions
    {
        public static void SendErrorsTo(this BusConfiguration cfg, string uriString)
        {
            try
            {
                var uri = new Uri(uriString);
                cfg.SendErrorsTo(uri);
            }
            catch (UriFormatException ex)
            {
                throw new ConfigurationException("The Uri for the error endpoint is invalid: " + uriString, ex);
            }
        }

        public static void ReceiveFrom(this BusConfiguration cfg, string uriString)
        {
            try
            {
                var uri = new Uri(uriString);
                cfg.ReceiveFrom(uri);
            }
            catch (UriFormatException ex)
            {
                throw new ConfigurationException("The Uri for the error endpoint is invalid: " + uriString, ex);
            }
        }
    }
}