using bookstoreagent.Books.Entities;
using bookstoreagent.Workflow.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace bookstoreagent.Infrastructure
{
    public class BookAgentDbContext:DbContext
    {
        public BookAgentDbContext(DbContextOptions<BookAgentDbContext> options) : base(options)         
        {
            
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
