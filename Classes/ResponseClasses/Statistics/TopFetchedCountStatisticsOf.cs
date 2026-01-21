using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Classes.ResponseClasses.Statistics;

public class TopFetchedCountStatisticsOf<T>(List<FetchCountStatisticsOf<T>> fetchCountStatistics): ICacheable
{
    public List<FetchCountStatisticsOf<T>> FetchCountStatistics { get; set; } = fetchCountStatistics;
}

