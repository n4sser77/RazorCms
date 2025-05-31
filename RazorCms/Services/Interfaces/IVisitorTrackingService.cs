// Assuming you're using Entity Framework Core for example
public interface IVisitorTrackingService
{
    public Task RecordVisitAsync(string visitorIdentifier,
                            string pageVisited = null,
                            string userAgent = null,
                            string referrer = null);

    public Task<int> GetTotalVisitCountAsync(string pageUrl = null);
    public Task<int> GetVisitCountForPeriodAsync(DateTime startDate, DateTime endDate, string pageUrl = null);
}