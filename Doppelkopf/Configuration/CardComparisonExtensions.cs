namespace Doppelkopf.Configuration;

public static class CardComparisonExtensions {
  public static CardComparison AsCardComparison(this int compare) => compare switch {
      > 0 => CardComparison.Higher,
      0 => CardComparison.Equal,
      < 0 => CardComparison.Lower
  };
}