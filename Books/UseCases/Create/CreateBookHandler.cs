using bookstoreagent.Books.Entities;
using bookstoreagent.Infrastructure;

namespace bookstoreagent.Books.UseCases.Create;

public sealed class CreateBookHandler(IBookRepository books, IVectorBookRepository vectorBooks)
{
    public async Task<Guid> Handle(CreateBookCommand command, CancellationToken ct)
    {
        var book = new Book(command.Title, command.Author, command.Genre, command.Year);
        await books.AddAsync(book, ct);
        await books.SaveChangesAsync(ct);
        try
        {
            await vectorBooks.Upsert(book, ct);
        }
        catch (Exception)
        {
            books.DeleteAsync(book, ct);
            await books.SaveChangesAsync(ct);
            throw;
        }
        return book.Id;
    }
}
