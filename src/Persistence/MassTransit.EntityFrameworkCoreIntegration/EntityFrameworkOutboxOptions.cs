namespace MassTransit
{
    using System.Data;


    public class EntityFrameworkOutboxOptions
    {
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Serializable;
    }
}
