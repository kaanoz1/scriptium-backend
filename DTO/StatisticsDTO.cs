namespace DTO;

public class GeneralStatisticsDTO
{
    public required int UserCount { get; set; }
    public required int RequestCount { get; set; }
    public required int DailyActiveUserCount { get; set; }
    public required int TotalSavedVerse { get; set; }
    
}

public class StatisticsOf<T>
{
    public required T Data { get; set; }
    public required Dictionary<DateOnly, int> dayDictionary { get; set; }
}
