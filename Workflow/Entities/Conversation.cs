using Microsoft.Extensions.AI;
using Microsoft.ML.Tokenizers;
using System.Reflection.Metadata.Ecma335;

namespace bookstoreagent.Workflow.Entities
{
    public class Conversation
    {
        public Guid Id { get; private set; }
        private readonly List<ConversationMessage> _messages = [];
        public IReadOnlyList<ConversationMessage> Messages => _messages.OrderBy(x=>x.CreatedAt).ToList().AsReadOnly();
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string? AgentSessionState { get; private set; }

        private Conversation()
        {
            
        }
        public Conversation(Guid id, List<ConversationMessage> messages)
        {
            Id = id;
            _messages = messages;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public void AddMessage(string agent,ChatRole role,IList<AIContent> contents,DateTime createdAt) {
            _messages.Add(new ConversationMessage(this,agent,role,contents,createdAt));
            UpdatedAt = createdAt;
        }

        public void SetAgentSessionState(string sessionState, DateTime updatedAt)
        {
            AgentSessionState = sessionState;
            UpdatedAt = updatedAt;
        }
    }

    public sealed class ConversationMessage
    {
        public Guid Id { get;private set; }
        public string Agent { get;private set; }
        public ChatRole Role { get;private set; }
        public DateTime CreatedAt { get; private set; }
        public IList<AIContent> Contents { get; private set; }
        public Conversation Conversation { get;private set; }
        private ConversationMessage()
        {            
        }

        public ConversationMessage(Conversation conversation, string agent, ChatRole role, IList<AIContent> contents,DateTime createdAt)
        {
            Id = Guid.NewGuid();
            Conversation = conversation;
            Agent = agent;
            Role = role;
            Contents = contents;
            CreatedAt = createdAt;
        }
    } 
}
