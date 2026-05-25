using bookstoreagent.Books.Entities;
using Microsoft.EntityFrameworkCore;

namespace bookstoreagent.Infrastructure;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    void DeleteAsync(Book book, CancellationToken ct);
    Task<List<Book>> GetByIdsAsync(List<Guid> ids, CancellationToken ct);
}

public sealed class EfBookRepository(BookAgentDbContext dbContext) : IBookRepository
{
    private readonly DbSet<Book> _entities = dbContext.Books;

    public Task AddAsync(Book book, CancellationToken ct)
    {
        return _entities.AddAsync(book, ct).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);

    public void DeleteAsync(Book book, CancellationToken ct)
    {
        _entities.Remove(book);
    }

    public async Task<List<Book>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
    {
        var books = await _entities.Where(b => ids.Contains(b.Id)).AsNoTracking()
            .ToDictionaryAsync(b => b.Id, ct);
        return ids
            .Where(books.ContainsKey)
            .Select(id => books[id])
            .ToList();
    }
}