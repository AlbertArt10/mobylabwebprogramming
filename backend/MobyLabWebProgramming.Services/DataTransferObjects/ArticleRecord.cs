namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to transfer information about an article to the client application.
/// MatchName and AuthorName are taken from the related entities by the projection so the client doesn't need extra requests.
/// </summary>
public class ArticleRecord
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public string MatchName { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
