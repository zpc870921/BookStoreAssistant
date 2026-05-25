using bookstoreagent.Workflow.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bookstoreagent.Infrastructure.EntitiesDConfigurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.Property(x=>x.Id).ValueGeneratedNever();
            builder.HasMany(x=>x.Messages).WithOne(m=>m.Conversation);
        }
    }
}