using bookstoreagent.Workflow.Agents;
using Microsoft.Agents.AI.Workflows;

namespace bookstoreagent.Workflow
{
    public class OrderBookWorkflowBuilder(BookOrderAgentBuilder orderAgentBuilder, BookRecommendationAgentBuilder recommendationAgentBuilder)
    {
        public Microsoft.Agents.AI.Workflows.Workflow Build()
        {
            var recommendationAgent=recommendationAgentBuilder.Build();
            var bookOrderAgent=orderAgentBuilder.Build();
#pragma warning disable MAAIW001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
            var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(recommendationAgent)
                .WithHandoff(recommendationAgent, bookOrderAgent, "The user selected a specific book and is ready to place an order.")
                .WithHandoff(bookOrderAgent,recommendationAgent,"The user wants to change the selected book or ask for new recommendations.")
                .Build();
#pragma warning restore MAAIW001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
            return workflow;
        }
    }
}
