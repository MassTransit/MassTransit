namespace MassTransitBenchmark;

using System;
using NDesk.Options;


public class SqlOptionSet :
    OptionSet
{
    public SqlOptionSet()
    {
        Add<string>("h|host:", "The host name of the broker", x => Host = x);
        Add<string>("u|username:", "Username (if using basic credentials)", value => Username = value);
        Add<string>("p|password:", "Password (if using basic credentials)", value => Password = value);
        Add<string>("db|database:", "Database to use", value => Database = value);
        Add<string>("role:", "Database role to use", value => Role = value);
        Add<string>("schema:", "Schema to use", value => Schema = value);

        Host = "localhost";
        Username = "postgres";
        Password = "Password12!";
        Database = "benchmark";
        Role = "transport";
        Schema = "transport";
    }

    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Database { get; set; }
    public string Schema { get; set; }

    public void ShowOptions()
    {
        Console.WriteLine("Host: {0}", Host);
        Console.WriteLine("Username: {0}", Username);
        Console.WriteLine("Password: {0}", new string('*', (Password ?? "default").Length));
        Console.WriteLine("Database: {0}", Database);
        Console.WriteLine("Role: {0}", Role);
        Console.WriteLine("Schema: {0}", Schema);
    }
}
