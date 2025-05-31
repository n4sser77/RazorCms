namespace RazorCms.Models;

/// <summary>
/// Represents a single recorded visit for tracking purposes.
/// </summary>
public class VisitLog
{
    /// <summary>
    /// unique identifier for the visit record (Primary Key, auto-incrementing).
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// unique identifier for the visitor (e.g., hashed IP, user ID, or session ID).
    /// </summary>
    public string VisitorIdentifier { get; set; }

    /// <summary>
    /// UTC timestamp of the visit.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// URL of the page that was visited.
    /// </summary>
    public string PageVisited { get; set; }

    /// <summary>
    /// the user agent is string of the browser/device used for the visit.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// the referring URL that led to this visit.
    /// </summary>
    public string Referrer { get; set; }
}