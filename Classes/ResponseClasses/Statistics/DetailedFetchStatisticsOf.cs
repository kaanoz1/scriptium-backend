namespace ScriptiumBackend.Classes.ResponseClasses.Statistics;

public class DetailedFetchStatisticsOf<T>(T entity, List<DayRecord> dailyRecords)
{
    public T Entity { get; set; } = entity;
    public List<DayRecord> DailyRecords { get; set; } = dailyRecords;
}