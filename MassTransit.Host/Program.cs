namespace MassTransit.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Controller controller = new Controller();
            controller.Start(args);
        }
    }
}