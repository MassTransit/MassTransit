namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public class PostgresLockStatementFormatter :
        ILockStatementFormatter
    {
        public void Create(StringBuilder sb, string schema, string table)
        {
            sb.AppendFormat("SELECT * FROM \"{0}\".\"{1}\" WHERE ", schema, table);
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
            sb.AppendFormat(@"SELECT * FROM ""{0}"".""{1}"" ORDER BY ""{2}"" LIMIT 1 FOR UPDATE SKIP LOCKED", schema, table, columnName);
        }
    }
}
