using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace bookstoreagent.Infrastructure.Converters
{
    public class ContentsConverter():ValueConverter<IList<AIContent>,string>(contents=>JsonSerializer.Serialize(contents,Options), json=>JsonSerializer.Deserialize<IList<AIContent>>(json,Options))
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions(AIJsonUtilities.DefaultOptions)
        {
            AllowOutOfOrderMetadataProperties = true,
        };
    }
}
