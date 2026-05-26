using Microsoft.Agents.AI;

namespace bookstoreagent.Workflow.Agents;

public sealed class BookStoreContextProvider : AIContextProvider
{
    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new AIContext
        {
            Instructions = """
            Persist important user choices across turns through the active session.
            Do not invent book ids, order ids, inventory, delivery addresses, or payment details.
            Use the provided tools for recommendations and order placement whenever the required fields are available.
            If a required field is missing, ask for that field only.
            """
        });
    }
}
