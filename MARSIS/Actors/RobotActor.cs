using Akka.Actor;
using Akka.Event;

namespace MARSIS.Actors;

/// <summary>
/// A robot actor that moves randomly, collects resources, and logs its state on each tick.
/// </summary>
public sealed class RobotActor : ReceiveActor
{
    private static readonly Random _random = new();

    private int _xPosition;
    private int _yPosition;
    private int _resourceCount;
    private double _energy = 100.0;

    public RobotActor()
    {
        // Handle Tick messages with the UpdateBehavior method.
        Receive<Tick>(_ => UpdateBehavior());
    }

    /// <summary>
    /// Updates the robot's state once per tick.
    /// </summary>
    private void UpdateBehavior()
    {
        DepleteEnergy();
        MoveRandomly();
        TryCollectResource();

        LogCurrentState();
    }

    /// <summary>
    /// Gradually reduces the robot's energy.
    /// </summary>
    private void DepleteEnergy()
    {
        _energy = Math.Max(_energy - 0.5, 0.0);
    }

    /// <summary>
    /// Moves the robot in a random direction by 1 unit.
    /// </summary>
    private void MoveRandomly()
    {
        _xPosition += _random.Next(-1, 2);
        _yPosition += _random.Next(-1, 2);
    }

    /// <summary>
    /// Attempts to collect a resource, which replenishes some energy.
    /// </summary>
    private void TryCollectResource()
    {
        // 30% chance per tick
        if (_random.NextDouble() < 0.3)
        {
            _resourceCount++;
            _energy = Math.Min(_energy + 5.0, 100.0);
        }
    }

    /// <summary>
    /// Logs the current internal state of the robot.
    /// </summary>
    private void LogCurrentState()
    {
        var logger = Context.GetLogger();
        logger.Info($"\n{Self.Path.Name} => Position:({_xPosition},{_yPosition}) | " +
                    $"Resources:{_resourceCount} | Energy:{_energy:F1}");
    }

    /// <summary>
    /// Factory method for creating <see cref="RobotActor"/> props.
    /// </summary>
    public static Props Props() => Akka.Actor.Props.Create<RobotActor>();
}
