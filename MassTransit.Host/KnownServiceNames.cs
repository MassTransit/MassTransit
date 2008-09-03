namespace MassTransit.Host
{
    public static class KnownServiceNames
    {
        public static string Msmq
        {
            get
            {
                return "MSMQ";
            }
        }

        public static string SqlServer
        {
            get
            {
                return "MSSQLSERVER";
            }
        }
    }
}