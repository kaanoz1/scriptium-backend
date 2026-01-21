using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Classes.ResponseClasses.Statistics;

public class TopDetailedStatisticsOf<T>(List<DetailedFetchStatisticsOf<T>> statistics): ICacheable
{
    public List<DetailedFetchStatisticsOf<T>> Statistics { get; set; } = statistics;
}