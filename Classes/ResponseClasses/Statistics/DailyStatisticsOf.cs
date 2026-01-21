using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Classes.ResponseClasses.Statistics;

public class DailyStatisticsOf<T>(T entity, List<DayRecord> records): ICacheable
{
    public T Entity { get; set; } = entity;

    public List<DayRecord> Records { get; set; } = records;
}