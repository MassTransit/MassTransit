namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public interface ILockStatementFormatter
    {
        void Create(StringBuilder sb, string schema, string table);
        void AppendColumn(StringBuilder sb, int index, string columnName);
        void Complete(StringBuilder sb);
    }
}
