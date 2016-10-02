namespace Repo1.Core.ns12.Helpers.DecimalExtensions
{
    public static class CommonDecimalExtensions
    {
        public static decimal PercentOf
            (this decimal numerator, decimal denominator)
            => denominator == 0 ? 0
            : (numerator / denominator) * 100M;

        public static decimal? PercentOf
            (this decimal? numerator, decimal? denominator)
            => numerator?.PercentOf(denominator ?? 0);
    }
}
