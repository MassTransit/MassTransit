namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Text;


    public class OracleLockStatementFormatter :
        ILockStatementFormatter
    {
        public void Create(StringBuilder sb, string schema, string table)
        {
            sb.AppendFormat("SELECT * FROM {0} WHERE ", FormatTableName(schema, table));
        }

        public void AppendColumn(StringBuilder sb, int index, string columnName)
        {
            if (index == 0)
                sb.AppendFormat("\"{0}\" = :p0", columnName);
            else
                sb.AppendFormat(" AND \"{0}\" = :p{1}", columnName, index);
        }

        public void Complete(StringBuilder sb)
        {
            // Attaching the FOR UPDATE makes the query fail with ORA-00907: Missing right parenthesis
            // I think the problem is that the query is wrapped in a subquery and the FOR UPDATE cannot be
            // in subquery in Oracle.

            //sb.Append(" FOR UPDATE");
        }

        public void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName)
        {
            var tabName = FormatTableName(schema, table);
            var s = $"SELECT * FROM {tabName}"
                + $" WHERE rowid=(SELECT \"mtl1\".rowid FROM {tabName} \"mtl1\""
                + $" ORDER BY \"mtl1\".\"" + columnName + "\""
                + " FETCH FIRST 1 ROW ONLY) FOR UPDATE SKIP LOCKED";
            sb.Append(s);
        }

        static string FormatTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema) ? $"\"{table}\"" : $"\"{schema}\".\"{table}\"";
        }
    }
}
