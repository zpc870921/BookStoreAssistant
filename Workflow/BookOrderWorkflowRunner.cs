using bookstoreagent.Infrastructure;
using bookstoreagent.Workflow.Agents;
using bookstoreagent.Workflow.Entities;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace bookstoreagent.Workflow
{
    public class BookOrderWorkflowRunner(OrderBookWorkflowBuilder builder, IConversationStore conversationStore)
    {
        public async Task<string> RunAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
        {
            var conversation = await conversationStore.GetOrCreateAsync(conversationId);
            var lastAgent = conversation.Messages.LastOrDefault()?.Agent ?? BookRecommendationAgentBuilder.Name;
            conversation.AddMessage(lastAgent, ChatRole.User, new List<AIContent> { new TextContent(message) },DateTime.UtcNow );

            var initMessageCount=conversation.Messages.Count;

            var workflow = builder.Build(lastAgent);
            var response = await InProcessExecution.RunAsync(
                workflow,
                input:conversation.Messages.Select(m=>new ChatMessage(m.Role, m.Contents)
                {
                    AuthorName=m.Agent,
                    CreatedAt=m.CreatedAt
                }).ToList(),
                conversationId.ToString(),
                cancellationToken);

            var outputEvent = response.OutgoingEvents.LastOrDefault(e => e is WorkflowOutputEvent);

            if (outputEvent is not WorkflowOutputEvent { Data: List<ChatMessage> { Count: > 0 } messages })
            {
                throw new Exception("Agent output is empty");
            }

            var now = DateTime.UtcNow;
            foreach (var item in messages.Skip(initMessageCount))
            {
                conversation.AddMessage(item.AuthorName, item.Role, item.Contents,now);
                now = now.AddMilliseconds(1);
            }
            await conversationStore.SaveChangesAsync();
            return (conversation.Messages.Last().Contents.First() as TextContent)!.Text;
        }
    }
}
