namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public class SqlServerLockStatementFormatter :
        ILockStatementFormatter
    {
        public void Create(StringBuilder sb, string schema, string table)
        {
            sb.AppendFormat("SELECT * FROM {0} WITH (UPDLOCK, ROWLOCK, SERIALIZABLE) WHERE ", GetSchemaTableName(schema, table));
        }

        public void AppendColumn(StringBuilder sb, int index, string columnName)
        {
            if (index == 0)
                sb.AppendFormat("{0} = @p0", columnName);
            else
                sb.AppendFormat(" AND {0} = @p{1}", columnName, index);
        }

        public void Complete(StringBuilder sb)
        {
        }

        public void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName)
        {
            sb.AppendFormat(@"SELECT TOP 1 * FROM {0} WITH (UPDLOCK, ROWLOCK, READPAST) ORDER BY {1}", GetSchemaTableName(schema, table), columnName);
        }

        private static string GetSchemaTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema) ? $"{table}" : $"[{schema}].{table}";
        }
    }
}
