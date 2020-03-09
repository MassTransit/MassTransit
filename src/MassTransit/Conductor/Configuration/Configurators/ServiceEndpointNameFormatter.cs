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

        public string InstanceEndpointName(string instanceName, IEndpointNameFormatter formatter)
        {
            var sb = new StringBuilder(instanceName.Length + 9);

            sb.Append("Instance");
            sb.Append('_');
            sb.Append(instanceName);

            return formatter.SanitizeName(sb.ToString());
        }

        public string InstanceServiceEndpointName(string endpointName, string instanceName, IEndpointNameFormatter formatter)
        {
            var sb = new StringBuilder(endpointName.Length + instanceName.Length + 9);

            sb.Append(endpointName);
            sb.Append("Instance");
            sb.Append('_');
            sb.Append(instanceName);

            return formatter.SanitizeName(sb.ToString());
        }

        public string ServiceControlEndpointName(string endpointName, IEndpointNameFormatter formatter)
        {
            return formatter.SanitizeName(endpointName + "Control");
        }

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
