using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

/// <summary>
/// This is a simple specification to filter the article entities from the database via the constructors.
/// The specification will extract the raw entities from the database without a projection, it is used when the entity needs to be modified.
/// </summary>
public sealed class ArticleSpec : Specification<Article>
{
    public ArticleSpec(Guid id) => Query.Where(e => e.Id == id);
}
