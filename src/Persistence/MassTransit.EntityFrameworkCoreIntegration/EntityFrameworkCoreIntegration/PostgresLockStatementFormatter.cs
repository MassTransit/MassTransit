namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public class PostgresLockStatementFormatter :
        ILockStatementFormatter
    {
        public void Create(StringBuilder sb, string schema, string table)
        {
            sb.AppendFormat("SELECT *, xmin FROM {0} WHERE ", FormatTableName(schema, table));
        }

        public void AppendColumn(StringBuilder sb, int index, string columnName)
        {
            if (index == 0)
                sb.AppendFormat("\"{0}\" = @p0", columnName);
            else
                sb.AppendFormat(" AND \"{0}\" = @p{1}", columnName, index);
        }

        public void Complete(StringBuilder sb)
        {
            sb.Append(" FOR UPDATE");
        }

        public void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName)
        {
            sb.AppendFormat(@"SELECT *, xmin FROM {0} ORDER BY ""{1}"" LIMIT 1 FOR UPDATE SKIP LOCKED", FormatTableName(schema, table), columnName);
        }

        public void CreateBulkOutboxStatement(StringBuilder sb, string outboxStateSchema, string outboxStateTable, string[] outboxStateColumnNames,
            string outboxMessageSchema, string outboxMessageTable, string[] outboxMessageColumnNames, int limit)
        {
            var stateTable = FormatTableName(outboxStateSchema, outboxStateTable);
            var stateOutboxId = $"\"{outboxStateColumnNames[0]}\"";
            var stateCreated = $"\"{outboxStateColumnNames[1]}\"";
            var messagesTable = FormatTableName(outboxMessageSchema, outboxMessageTable);
            var messagesOutboxId = $"\"{outboxMessageColumnNames[0]}\"";

            var sql = $"""
                WITH deleted_main AS (
                	DELETE FROM {stateTable}
                	WHERE {stateOutboxId} IN (
                		SELECT {stateOutboxId} FROM {stateTable}
                		ORDER BY {stateCreated}
                		LIMIT {limit}
                		for update skip locked
                	)
                	RETURNING {stateOutboxId}
                ),
                deleted_sub AS (
                	DELETE FROM {messagesTable}
                	WHERE {messagesOutboxId} IN (SELECT {stateOutboxId} FROM deleted_main)
                	RETURNING *
                )
                SELECT * FROM deleted_sub;
                """;

            sb.Append(sql);
        }

        static string FormatTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema) ? $"\"{table}\"" : $"\"{schema}\".\"{table}\"";
        }
    }
}
