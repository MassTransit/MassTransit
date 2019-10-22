namespace MassTransit.Conductor.Configuration.Configurators
{
    using System.Text;
    using Util;


    public class ServiceEndpointNameFormatter
    {
        ServiceEndpointNameFormatter()
        {
        }

        public static ServiceEndpointNameFormatter Instance { get; } = new ServiceEndpointNameFormatter();

        public string EndpointName(NewId instanceId, string tag = null)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "instance";

            var sb = new StringBuilder(tag.Length + 35);

            sb.Append(tag);

            sb.Append('_');
            sb.Append(instanceId.ToString(FormatUtil.Formatter));

            return sb.ToString();
        }
    }
}
