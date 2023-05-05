using Doppelkopf.Configuration;
using Doppelkopf.GameFinding;
using Doppelkopf.Tricks;

namespace Doppelkopf.Match;

public sealed record Match(IRules Rules, Auction Auction, TrickTaking? TrickTaking);