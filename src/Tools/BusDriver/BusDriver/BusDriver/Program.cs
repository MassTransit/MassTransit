namespace BusDriver
{
    using System;
    using System.Linq;
    using Commands;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;
    using Magnum;
    using Magnum.CommandLineParser;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));
        private static ConsoleAppender _appender;

        private static void Main(string[] args)
        {
            BootstrapLogger();

            _log.Info("Starting up ");

            Console.WriteLine("Hail to the bus driver, bus driver man!");


            // startup


            while (ProcessCommand())
            {
            }


            Console.WriteLine("Shutting down...");
        }

        private static bool ProcessCommand()
        {
            try
            {
                Console.Write("$ ");

                string line = Console.ReadLine();

                var parser = new MonadicCommandLineParser();

                var elements = parser.Parse(line);

                _log.Debug("Parsed: " + string.Join(", ", elements.Select(x => x.ToString()).ToArray()));

                var element = elements.First();

                if (element is IArgumentElement)
                {
                    var argument = element as IArgumentElement;

                    if (argument.Id == "quit" || argument.Id == "exit")
                        return false;

                    if (argument.Id == "create")
                    {
                        var command = new CreateCommand(elements.Skip(1));
                    }

                    if (argument.Id == "count")
                    {
                        var command = new CountEndpointCommand(elements.Skip(1));
                    }

                    if(argument.Id == "send")
                    {
                        new SendTextCommand(elements.Skip(1));
                    }

                    if(argument.Id == "receive")
                    {
                        new ReceiveCommand(elements.Skip(1));
                    }

                    if (argument.Id == "move")
                        new MoveCommand(elements.Skip(1));
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return true;
        }

        private static void BootstrapLogger()
        {
            _appender = new ConsoleAppender();
            _appender.Layout = new SimpleLayout();

            BasicConfigurator.Configure(_appender);
        }
    }
}