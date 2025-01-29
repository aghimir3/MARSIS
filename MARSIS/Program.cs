using Akka.Actor;
using Akka.Configuration;
using MARSIS.Actors;
using Serilog;

namespace MARSIS;

public static class Program
{
    public static void Main(string[] args)
    {
        // Setup Serilog for console logs
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        // 1) Load HOCON from external file "app.conf"
        var hocon = File.ReadAllText("app.conf");
        var config = ConfigurationFactory.ParseString(hocon);

        // 2) Create the ActorSystem with the name "MARSIS"
        using var system = ActorSystem.Create("MARSIS", config);

        // 3) Extract "swarm-size" and "tick-interval-seconds" from config
        var swarmSize = config.GetInt("marsis.simulation.swarm-size");
        var tickIntervalSeconds = config.GetInt("marsis.simulation.tick-interval-seconds");
        var tickInterval = TimeSpan.FromSeconds(tickIntervalSeconds);

        // 4) Spawn the SwarmManager actor using the configured swarm size       
        var swarmManager = system.ActorOf(
            SwarmManager.Props(swarmSize, tickInterval),
            "swarm-manager"
        );

        Console.WriteLine("MARSIS Simulation Running. Press any key to exit...");
        Console.ReadKey();
    }
}
