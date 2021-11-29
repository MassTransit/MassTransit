namespace MassTransit.ActiveMqTransport
{
    using System.Text;


    public static class ActiveMqHostSettingsExtensions
    {
        public static string ToDescription(this ActiveMqHostSettings settings)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(settings.Username))
                sb.Append(settings.Username).Append('@');

            sb.Append(settings.Host);

            if (settings.Port != -1)
                sb.Append(':').Append(settings.Port);

            return sb.ToString();
        }
    }
}
