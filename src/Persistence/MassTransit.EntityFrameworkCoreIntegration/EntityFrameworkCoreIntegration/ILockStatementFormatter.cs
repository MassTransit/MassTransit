namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public interface ILockStatementFormatter
    {
        void Create(StringBuilder sb, string schema, string table);
        void AppendColumn(StringBuilder sb, int index, string columnName);
        void Complete(StringBuilder sb);

        void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName);

        void CreateBulkOutboxStatement(StringBuilder sb, string outboxStateSchema, string outboxStateTable, string[] outboxStateColumnNames,
            string outboxMessageSchema, string outboxMessageTable, string[] outboxMessageColumnNames, int limit);
    }
}
