namespace Doppelkopf.Configuration;

public interface IRules
{
  GameModeCollection Modes { get; }
  RuleSet RuleSet { get; }
}
