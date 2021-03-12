namespace TheGameApp
{
    /// <summary>
    /// The result from the <see cref="SimulationRunner" />.
    /// </summary>
    internal record SimulationResult(int    MinNumberOfCardsLeft,     int    MaxNumberOfCardsLeft,
                                     double AverageNumberOfCardsLeft, double PercentageExcellentResult,
                                     double PercentagePerfectResult)
    {
    }
}