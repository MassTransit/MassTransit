namespace MassTransit.Serialization
{
    public interface ISecureKeyProvider :
        IProbeSite
    {
        byte[] GetKey(Headers headers);
    }
}
