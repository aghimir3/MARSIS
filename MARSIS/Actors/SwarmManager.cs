using Akka.Actor;

namespace MARSIS.Actors;

/// <summary>
/// Manages a swarm of <see cref="RobotActor"/> instances,
/// forwarding periodic ticks to each robot.
/// </summary>
public sealed class SwarmManager : ReceiveActor, IWithTimers
{
    private readonly List<IActorRef> _robotActors = [];
    private readonly int _swarmSize;
    private readonly TimeSpan _tickInterval;

    public ITimerScheduler Timers { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwarmManager"/>.
    /// </summary>
    /// <param name="swarmSize">Number of robots in the swarm.</param>
    public SwarmManager(int swarmSize, TimeSpan tickInterval)
    {
        _swarmSize = swarmSize;
        _tickInterval = tickInterval;

        // Set up message handling.
        Receive<Tick>(_ => HandleTick());
    }

    /// <summary>
    /// Akka.NET lifecycle method called when the actor is started.
    /// </summary>
    protected override void PreStart()
    {
        base.PreStart();
        InitializeSwarm();
        StartPeriodicTick();
    }

    /// <summary>
    /// Creates the swarm of robot actors.
    /// </summary>
    private void InitializeSwarm()
    {
        foreach (var i in Enumerable.Range(1, _swarmSize))
        {
            var robot = Context.ActorOf(RobotActor.Props(), $"robot-{i}");
            _robotActors.Add(robot);
        }
    }

    /// <summary>
    /// Sets up a periodic timer that sends <see cref="Tick"/> messages to self.
    /// </summary>
    private void StartPeriodicTick()
    {
        Timers.StartPeriodicTimer(
            key: "tick-timer",
            msg: Tick.Instance,
            initialDelay: _tickInterval,
            interval: _tickInterval);
    }

    /// <summary>
    /// Forwards the tick to all robot actors in the swarm.
    /// </summary>
    private void HandleTick()
    {
        foreach (var robot in _robotActors)
        {
            robot.Tell(Tick.Instance);
        }
    }

    /// <summary>
    /// Factory method for creating <see cref="SwarmManager"/> actor props.
    /// </summary>
    public static Props Props(int swarmSize, TimeSpan tickInterval) =>
        Akka.Actor.Props.Create(() => new SwarmManager(swarmSize, tickInterval));
}
