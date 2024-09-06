namespace MassTransit.AccessToken
{

    using Amazon;
    using Amazon.RDS.Util;

    sealed class RdsTokenGenerator(RegionEndpoint regionEndpoint) : ITokenGenerator
    {
        public string GenerateToken(SqlTransportOptions options)
        {
            return RDSAuthTokenGenerator.GenerateAuthToken(regionEndpoint, options.Host, options.Port.Value,
                options.Username);
        }
    }

}



