using enova_academy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace enova_academy.Data.Config;

public class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Status)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.ToTable("Enrollments");
    }
}
