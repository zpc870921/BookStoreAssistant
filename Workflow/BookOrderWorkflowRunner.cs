using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;

namespace bookstoreagent.Workflow
{
    public class BookOrderWorkflowRunner(OrderBookWorkflowBuilder builder, AgentSessionStore sessionStore)
    {
        public async Task<string> RunAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
        {
            var workflowAgent = builder.Build().AsAIAgent(
                id: "BookOrderWorkflow",
                name: "BookOrderWorkflow",
                description: "Routes a bookstore conversation between recommendation and order agents.",
                executionEnvironment: InProcessExecution.Default,
                includeExceptionDetails: false,
                includeWorkflowOutputsInResponse: true);

            var hostedAgent = new AIHostAgent(workflowAgent, sessionStore);
            var session = await hostedAgent.GetOrCreateSessionAsync(conversationId.ToString("D"), cancellationToken);
            var response = await hostedAgent.RunAsync(message, session, cancellationToken: cancellationToken);

            await hostedAgent.SaveSessionAsync(conversationId.ToString("D"), session, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.Text))
            {
                throw new InvalidOperationException("Agent output is empty.");
            }

            return response.Text;
        }
    }
}
