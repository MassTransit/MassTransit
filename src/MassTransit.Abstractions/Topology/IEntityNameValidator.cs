namespace MassTransit
{
    public interface IEntityNameValidator
    {
        bool IsValidEntityName(string name);

        void ThrowIfInvalidEntityName(string name);
    }
}
