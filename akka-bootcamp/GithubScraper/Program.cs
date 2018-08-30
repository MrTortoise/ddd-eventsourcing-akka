using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Serilog;

namespace GithubScraper
{
    class Program
    {
        static void Main(string[] args)
        {
          

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            Serilog.Log.Logger = logger;
            
            var cfg = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            var system = ActorSystem.Create("GitHubScraper", cfg);

            Action<string, string> writer = (source, authStatus) =>
            {
                Console.WriteLine($"{source}: {authStatus}");
            };

            var serviceActor = system.ActorOf(ServiceActor.CreateProps(writer, new FileInfo("./data.json")), ServiceActor.Name);
            serviceActor.Tell(new ServiceActor.Start());

            var input = Console.ReadLine();
            while (input != "quit")
            {
                serviceActor.Tell(new Input(input));
                input = Console.ReadLine();
            }
        }
    }
    


}