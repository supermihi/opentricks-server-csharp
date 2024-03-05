namespace Doppelkopf.Core.Contracts;

/// <summary>
/// Defines available game modes.
/// </summary>
public record Modes(IReadOnlyCollection<IHold> Holds, INormalGameProvider NormalGame);
