namespace Doppelkopf.Table;

public readonly record struct Seat(int Position)
{
  public Seat Next(int numSeats) => new((Position + 1) % numSeats);
}
