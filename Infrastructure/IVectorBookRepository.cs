using bookstoreagent.Books.Entities;
using bookstoreagent.Infrastructure.Vectors;

namespace bookstoreagent.Infrastructure;

public interface IVectorBookRepository
{
    Task Upsert(Book book, CancellationToken ct);
    Task<List<Guid>> Query(string queryString, CancellationToken ct);
}

public class VectorBookRepository(IVectorDbClient client, IEmbedding embedding) : IVectorBookRepository
{
    public async Task Upsert(Book book, CancellationToken ct)
    {
        var vector = await embedding.GetEmbeddingAsync($"Name: {book.Title}, Writer: {book.Author}, genre: {book.Genre}, year: {book.Year}", ct);
        
        var point = new VectorItem(
            book.Id,
            vector,
            new Dictionary<string, string> { { "type", nameof(Book) } }
        );

        await client.Upsert([point], ct);
    }
    
    public async Task<List<Guid>> Query(string queryString, CancellationToken ct)
    {
        var vector = await embedding.GetEmbeddingAsync(queryString, ct);
        return (await client.Query(vector, nameof(Book), ct)).Select(b => b.Id).ToList();
    }
}