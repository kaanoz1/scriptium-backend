namespace ScriptiumBackend.Classes.ResponseClasses.Statistics;

public class FetchCountStatisticsOf<T>(T entity, int count)
{
    public T Entity { get; set; } = entity;
    public int Count { get; set; } = count;
}