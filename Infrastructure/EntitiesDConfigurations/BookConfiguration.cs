using bookstoreagent.Books.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bookstoreagent.Infrastructure.EntitiesDConfigurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.Title).IsRequired().HasMaxLength(256);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(256);
        builder.Property(b => b.Genre).IsRequired().HasMaxLength(128);
        builder.Property(b => b.Year).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
    }
}
