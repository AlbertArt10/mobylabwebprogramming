namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to transfer the state of the likes of an article to the client application.
/// Note that IsLikedByCurrentUser depends on who is asking, so the same article gives a different answer for each user.
/// </summary>
public class ArticleLikeSummaryRecord
{
    public Guid ArticleId { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
}
