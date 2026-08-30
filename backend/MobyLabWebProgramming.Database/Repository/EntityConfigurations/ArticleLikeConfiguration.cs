using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the ArticleLike entity, the join table of the Many-To-Many relation between users and articles.
/// The relation is declared as two One-To-Many relations towards this table, which is how a join table with its own columns is mapped.
/// </summary>
public class ArticleLikeConfiguration : IEntityTypeConfiguration<ArticleLike>
{
    public void Configure(EntityTypeBuilder<ArticleLike> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasIndex(e => new { e.UserId, e.ArticleId }) // Note that the index is on the pair of columns and not on a single one,
            .IsUnique(); // this is what stops a user from liking the same article twice while still allowing many likes for each side.

        builder.HasOne(e => e.User)
            .WithMany(e => e.ArticleLikes)
            .HasForeignKey(e => e.UserId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // If the user is deleted their likes are deleted with them.

        builder.HasOne(e => e.Article)
            .WithMany(e => e.ArticleLikes)
            .HasForeignKey(e => e.ArticleId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // If the article is deleted its likes are deleted with it.
    }
}
