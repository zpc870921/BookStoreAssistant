using Microsoft.Extensions.AI;

namespace bookstoreagent.Infrastructure.Vectors
{
    public interface IEmbedding
    {
        Task<float[]> GetEmbeddingAsync(string input,CancellationToken ct);
    }

    public class OpenAiEmbedding(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : IEmbedding
    {

        public async Task<float[]> GetEmbeddingAsync(string input, CancellationToken ct)
        {
            var result=await embeddingGenerator.GenerateAsync(input,cancellationToken:ct);
            return result.Vector.ToArray();
        }
    }
}
