namespace MassTransit.AccessToken
{
    public interface ITokenGenerator
    {
        string GenerateToken(SqlTransportOptions options);
    }

}

