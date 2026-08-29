using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the Article entity.
/// Note that two relations are declared here, one towards the match and one towards the author, both of them One-To-Many.
/// </summary>
public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.Title)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.Content)
            .HasMaxLength(4095) // The content is longer than a usual text column, an article needs the room.
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.Match) // An article is written about one match and a match can have many articles.
            .WithMany(e => e.Articles)
            .HasForeignKey(e => e.MatchId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // If the match is deleted its articles are deleted with it.

        builder.HasOne(e => e.Author) // An article has one author and a user can write many articles.
            .WithMany(e => e.Articles)
            .HasForeignKey(e => e.AuthorId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
