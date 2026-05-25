using bookstoreagent.Workflow.Entities;
using Microsoft.EntityFrameworkCore;

namespace bookstoreagent.Infrastructure
{
    public interface IConversationStore
    {
        Task<Conversation> GetOrCreateAsync(Guid conversationId);
        Task SaveChangesAsync();
    }

    public class InMemoryConversationStore : IConversationStore
    {
        private readonly Dictionary<Guid, Conversation> _conversations = new();
        public async Task<Conversation> GetOrCreateAsync(Guid conversationId)
        {
            if (_conversations.TryGetValue(conversationId, out var existing))
            {
                return existing;
            }
            var conversation = new Conversation(conversationId, new List<ConversationMessage>());
            _conversations[conversationId] = conversation;
            return conversation;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    public class DbConversationStore : IConversationStore
    {
        private readonly BookAgentDbContext _dbContext;
        public DbConversationStore(BookAgentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Conversation> GetOrCreateAsync(Guid conversationId)
        {
            var conversation = await _dbContext.Conversations
                .Include(c=>c.Messages)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
            if (conversation != null)
            {
                return conversation;
            }
            conversation = new Conversation(conversationId, new List<ConversationMessage>());
            await _dbContext.Conversations.AddAsync(conversation);
            await _dbContext.SaveChangesAsync();
            return conversation;
        }
        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
