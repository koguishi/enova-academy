using enova_academy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace enova_academy.Data.Config;

public class CourseConfig : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasColumnType("varchar")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Price)
            .HasColumnType("decimal(10,2)");

        builder.Property(m => m.Capacity)
            .HasColumnType("integer");

        builder.HasIndex(c => new { c.Slug })
           .IsUnique(true);

        builder.ToTable("Courses");
    }
}
