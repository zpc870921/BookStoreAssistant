using bookstoreagent.Workflow.Entities;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace bookstoreagent.Infrastructure;

public sealed class EfAgentSessionStore(BookAgentDbContext dbContext) : AgentSessionStore
{
    public override async ValueTask SaveSessionAsync(
        AIAgent agent,
        string conversationId,
        AgentSession session,
        CancellationToken cancellationToken)
    {
        var conversation = await GetOrCreateConversationAsync(conversationId, cancellationToken);
        var state = await agent.SerializeSessionAsync(session, cancellationToken: cancellationToken);

        conversation.SetAgentSessionState(state.GetRawText(), DateTime.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public override async ValueTask<AgentSession> GetSessionAsync(
        AIAgent agent,
        string conversationId,
        CancellationToken cancellationToken)
    {
        var parsedConversationId = ParseConversationId(conversationId);
        var conversation = await dbContext.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == parsedConversationId, cancellationToken);

        if (string.IsNullOrWhiteSpace(conversation?.AgentSessionState))
        {
            return await agent.CreateSessionAsync(cancellationToken);
        }

        using var document = JsonDocument.Parse(conversation.AgentSessionState);
        return await agent.DeserializeSessionAsync(document.RootElement, cancellationToken: cancellationToken);
    }

    private async Task<Conversation> GetOrCreateConversationAsync(string conversationId, CancellationToken cancellationToken)
    {
        var parsedConversationId = ParseConversationId(conversationId);
        var conversation = await dbContext.Conversations
            .FirstOrDefaultAsync(c => c.Id == parsedConversationId, cancellationToken);

        if (conversation is not null)
        {
            return conversation;
        }

        conversation = new Conversation(parsedConversationId, []);
        await dbContext.Conversations.AddAsync(conversation, cancellationToken);
        return conversation;
    }

    private static Guid ParseConversationId(string conversationId)
    {
        if (Guid.TryParse(conversationId, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException("Conversation id must be a valid GUID.", nameof(conversationId));
    }
}
