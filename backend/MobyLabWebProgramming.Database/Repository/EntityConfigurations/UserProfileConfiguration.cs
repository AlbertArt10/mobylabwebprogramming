using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the UserProfile entity, here the One-To-One relation with the User entity is declared.
/// </summary>
public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.FavoriteTeam)
            .HasMaxLength(255)
            .IsRequired(false); // This specifies that this column can be null in the database.
        builder.Property(e => e.Country)
            .HasMaxLength(255)
            .IsRequired(false);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.User) // Note the difference from a One-To-Many relation, here it is WithOne instead of WithMany,
            .WithOne(e => e.Profile) // and the foreign key needs the type because the framework cannot tell on which of the two tables to put it.
            .HasForeignKey<UserProfile>(e => e.UserId)
            .HasPrincipalKey<User>(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // If the user is deleted its profile is deleted with it.
    }
}
