namespace MassTransit.AccessToken
{

    using System;
    using Amazon;


    public static class SqlTransportPasswordRetriever
    {
        public static string? GetPassword(SqlTransportOptions options)
        {
            if (options.AuthenticationMode == AuthenticationMode.Password)
                return options.Password;

            var tokenGenerator = options.TokenIssuerOptions.Issuer switch
            {
                TokenIssuer.AwsRds => new RdsTokenGenerator(RegionEndpoint.GetBySystemName(options.TokenIssuerOptions.AwsRdsRegionSystemName)),
                _ => throw new ArgumentOutOfRangeException(nameof(options.TokenIssuerOptions.Issuer), options.TokenIssuerOptions.Issuer, "This Token Issuer is not supported")
            };

            return tokenGenerator.GenerateToken(options);
        }

    }

}

