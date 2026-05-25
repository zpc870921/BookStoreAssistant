using bookstoreagent.Books.UseCases.Create;
using Microsoft.AspNetCore.Mvc;

namespace bookstoreagent.Controllers;

[ApiController]
[Route("api/books")]
public class BookController(CreateBookHandler createBookHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateBookCommand command, CancellationToken ct)
    {
        var bookId = await createBookHandler.Handle(command, ct);
        return Created(bookId.ToString(), bookId);
    }
}
