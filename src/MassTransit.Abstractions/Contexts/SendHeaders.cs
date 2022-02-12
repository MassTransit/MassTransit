namespace MassTransit
{
    public interface SendHeaders :
        Headers
    {
        void Set(string key, string? value);
        void Set(string key, object? value, bool overwrite = true);
    }
}
