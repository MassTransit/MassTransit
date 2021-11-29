namespace MassTransit
{
    public interface INewIdParser
    {
        NewId Parse(in string text);
    }
}
