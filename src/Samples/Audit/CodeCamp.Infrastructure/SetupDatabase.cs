namespace CodeCamp.Infrastructure
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using NHibernate.Cfg;
    using NHibernate.Dialect;
    using Npgsql;

    public class SetupDatabase
    {
        private string _sqlString = "Database=test;Server=localhost;Integrated Security=SSPI";
        private string _postgreSqlString = "User ID=test;Password=test;Host=localhost;Port=5432;Database=test;";
        private Configuration _cfg;
        IDictionary<string, string> sqlProps = new Dictionary<string, string> { { "dialect", typeof(MsSql2005Dialect).FullName } };
        IDictionary<string, string> pgsqlProps = new Dictionary<string, string> { { "dialect", typeof(PostgreSQL82Dialect).FullName } };

        public SetupDatabase()
        {
            _cfg = new Configuration();
        }

        //run with testdriven.net
        public void SetupMsSql()
        {
            _cfg.AddProperties(sqlProps);
            _cfg.AddAssembly(typeof(SetupDatabase).Assembly);

            var sql = _cfg.GenerateSchemaCreationScript(Dialect.GetDialect(sqlProps));
            ExecuteSql(sql, new SqlConnection(_sqlString));
        }

        //run with testdriven.net
        public void SetupPostgreSql()
        {
            _cfg.AddProperties(pgsqlProps);
            _cfg.AddAssembly(typeof(SetupDatabase).Assembly);

            var sql = _cfg.GenerateSchemaCreationScript(Dialect.GetDialect(pgsqlProps));
            ExecuteSql(sql, new NpgsqlConnection(_postgreSqlString));
        }

        public void ExecuteSql(string[] sql, IDbConnection conn)
        {
            using (conn)
            {
                conn.Open();
                foreach (var s in sql)
                {
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = s;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }


            }

        }
    }
}
