namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to add an article, note that it has no author property because the author is the user that makes the request.
/// </summary>
public record ArticleAddRecord(Guid MatchId, string Title, string Content);
