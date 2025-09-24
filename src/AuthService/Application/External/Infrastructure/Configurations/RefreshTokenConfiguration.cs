using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(r => r.Token)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(r => r.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(r => r.IpAddress)
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(r => r.Issued)
            .IsRequired();

        builder.Property(r => r.Expires)
            .IsRequired();
        
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .IsRequired();
    }
}