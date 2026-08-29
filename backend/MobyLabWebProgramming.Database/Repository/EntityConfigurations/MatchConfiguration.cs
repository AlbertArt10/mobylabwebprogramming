using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

/// <summary>
/// This is the entity configuration for the Match entity, here the One-To-Many relation with the Sport entity is declared.
/// </summary>
public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.Property(e => e.Id)
            .IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(e => e.HomeTeam)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.AwayTeam)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.MatchDate)
            .IsRequired();
        builder.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<MatchStatusEnum>()) // The enum is stored as a string in the database to keep the values readable.
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(e => e.HomeScore)
            .IsRequired(false); // The scores are known only after the match is played.
        builder.Property(e => e.AwayScore)
            .IsRequired(false);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.Sport) // Here the relation is declared, a match belongs to one sport and a sport has many matches.
            .WithMany(e => e.Matches)
            .HasForeignKey(e => e.SportId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
