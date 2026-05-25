namespace bookstoreagent.Books.Entities;

public sealed class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Genre { get; private set; }
    public int Year { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Book()
    {
    }

    public Book(string title, string author, string genre, int year)
    {
        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        Genre = genre;
        Year = year;
        CreatedAt = DateTime.UtcNow;
    }
}