using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a simple specification to filter the article like entities from the database via the constructors.
/// The first constructor selects all the likes of an article, the second one selects the like of a single user on that article.
/// </summary>
public sealed class ArticleLikeSpec : Specification<ArticleLike>
{
    public ArticleLikeSpec(Guid articleId) => Query.Where(e => e.ArticleId == articleId);

    public ArticleLikeSpec(Guid articleId, Guid userId) => Query.Where(e => e.ArticleId == articleId && e.UserId == userId);
}
