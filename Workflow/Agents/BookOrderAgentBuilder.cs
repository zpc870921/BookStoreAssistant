using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace bookstoreagent.Workflow.Agents
{
    public interface IBookOrderService
    {
        [Description("Order book")]
        Task<Guid> OrderBook([Description("Order book request details")]BookOrderRequest request,CancellationToken ct);
    }

    public class BookOrderService:IBookOrderService
    {
        public async Task<Guid> OrderBook(BookOrderRequest request,CancellationToken ct)
        {
            return Guid.NewGuid();
        }
    }

    public record BookOrderRequest(Guid BookId, string DeliveryAddress, string PaymentMethod);

    public class BookOrderAgentBuilder(IChatClient client,IBookOrderService bookOrderService)
    {
        public const string Name = "BookOrderAgent";
        public ChatClientAgent Build()
        {
            return client.AsAIAgent(new ChatClientAgentOptions
            {
                 Name=Name,
                 Description= "Collects order details and places a book order for the selected book.",
                 ChatHistoryProvider = new InMemoryChatHistoryProvider(new InMemoryChatHistoryProviderOptions
                 {
                     StateKey = Name
                 }),
                 AIContextProviders = [new BookStoreContextProvider()],
                 ChatOptions=new ChatOptions
                 {
                     Temperature = 0f,
                     Instructions= """
                        You are book shop assistant. You need to help the user to order selected book.
                        Selected BookId is available in prior conversation context after handoff.
                        You must collect the following information:
                        - DeliveryAddress
                        - PaymentMethod
                        When information is collected you must call OrderBook tool. And respond with order id. 
                     """,
                     Tools = [AIFunctionFactory.Create(bookOrderService.OrderBook,"OrderBook")]
                 }
            });
        }
    }
}
