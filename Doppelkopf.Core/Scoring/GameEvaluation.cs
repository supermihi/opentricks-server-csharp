using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public sealed record GameEvaluation(ByParty<int> Points, Party? Winner, ByPlayer<int> Score);
