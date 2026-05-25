using bookstoreagent.Infrastructure.Converters;
using bookstoreagent.Workflow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bookstoreagent.Infrastructure.EntitiesDConfigurations
{
    public class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
    {
        public void Configure(EntityTypeBuilder<ConversationMessage> builder)
        {
            builder.Property(x=>x.Id).ValueGeneratedNever();
            builder.Property(x=>x.Agent).HasColumnType("varchar(50)");
            builder.Property(x=>x.Role).HasConversion<ChatRoleConverter>().HasColumnType("varchar(32)");
            builder.Property(x=>x.Contents).HasConversion<ContentsConverter>().HasColumnType("json");
            builder.HasOne(x=>x.Conversation).WithMany(m=>m.Messages);
        }
    }
}