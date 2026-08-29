namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to transfer information about a sport to the client application.
/// Note that MatchCount is not a column in the database, it is computed by the projection so the client doesn't need a second request to count the matches.
/// </summary>
public class SportRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int MatchCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
