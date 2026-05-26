using bookstoreagent.Infrastructure;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace bookstoreagent.Workflow.Agents
{
    public interface IBookRecommendationService
    {
        [Description("Fetch Recommended Books")]
        Task<List<Book>> SuggestBook([Description("Book Preferences request details")]BookRecommendationRequest request,CancellationToken ct);
    }

    public class BookRecommendationRequest
    {
        public string Genre { get; set; }
        public string Author { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
    }

    public class BookRecommendationService(IVectorBookRepository vectorBooks, IBookRepository books) : IBookRecommendationService
    {
        public async Task< List<Book>> SuggestBook(BookRecommendationRequest request,CancellationToken ct)
        {
            //return new List<Book>
            //{
            //    new Book(Guid.NewGuid(), "New Dark Ages:Colony", "Max Kidruk", "Science Finction", 2022),
            //    new Book(Guid.NewGuid(), "New Dark Ages:Collapse", "Max Kidruk", "Science Finction", 2026)
            //};
            var bookIds = await vectorBooks.Query($"Genre: {request.Genre} Writer: {request.Author} Year: {request.MinYear}-{request.MaxYear}", ct);
            if (!bookIds.Any()) return [];
            var booksResult = await books.GetByIdsAsync(bookIds, ct);
            return booksResult.Select(b => new Book(b.Id, b.Title, b.Author, b.Genre, b.Year)).ToList();

        }
    }
    public record Book(Guid Id, string Title, string Author, string Genre, int Year);
    public class BookRecommendationAgentBuilder(IChatClient chatClient,IBookRecommendationService bookRecommendationService)
    {
        public const string Name = "BookRecommendationAgent";
        public ChatClientAgent Build()
        {
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                Name = Name,
                Description= "Collects user reading preferences,recommends books",
                ChatHistoryProvider = new InMemoryChatHistoryProvider(new InMemoryChatHistoryProviderOptions
                {
                    StateKey = Name
                }),
                AIContextProviders = [new BookStoreContextProvider()],
                ChatOptions = new Microsoft.Extensions.AI.ChatOptions
                {
                    Temperature = 0f,
                    Instructions = """
                    You are a book shop assistant. You need to collection information about person preferences. The information you need to collect one by one :
                    - genre
                    - writer
                    - year
                    When information is collected， You must call SuggestBook tool.
                    You will receive a list of books.Show the list to the user.
                    Ask the user which book they want to order.
                    """,
                    Tools = [AIFunctionFactory.Create(bookRecommendationService.SuggestBook,"SuggestBook")]
                }
            });
        }

        //public async Task<AgentSession> GetSession(AIAgent agent,Guid conversationId,CancellationToken ct)
        //{
        //    return await conversationSessionStore.GetOrCreate(conversationId,  () => agent.CreateSessionAsync(ct).AsTask());
        //}
    }
}
