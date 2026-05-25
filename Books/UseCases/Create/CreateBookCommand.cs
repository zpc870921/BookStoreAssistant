namespace bookstoreagent.Books.UseCases.Create;

public sealed record CreateBookCommand(string Title, string Author, string Genre, int Year);