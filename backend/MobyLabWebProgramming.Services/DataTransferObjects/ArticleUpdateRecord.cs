namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to update an article, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// The author cannot be changed, an article stays with the user that wrote it.
/// </summary>
public record ArticleUpdateRecord(Guid Id, Guid? MatchId = null, string? Title = null, string? Content = null);
