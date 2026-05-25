using System.Numerics;
using System.Text;
using System.Text.Json;

namespace bookstoreagent.Infrastructure.Vectors
{
    public interface IVectorDbClient
    {
        Task Upsert(List<VectorItem> points, CancellationToken ct);
        Task<List<ResultItem>> Query(
            float[] vector,
            string type,
            CancellationToken ct
        );
        Task Remove(string id, CancellationToken ct);
        Task CreateDefaultCollection();
    }

    public record VectorItem(Guid Id, float[] Vector,Dictionary<string,string> Payload);
    public record VectorResult(List<ResultItem> Result);
    public record ResultItem(Guid Id, float Score,Dictionary<string,string> Payload);

    public class QdrantClient(HttpClient httpClient, string apiUrl) : IVectorDbClient
    {
        private const int VectorSize = 1024;

        public async Task Upsert(List<VectorItem> points, CancellationToken cancellation)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { points },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await httpClient.PutAsync($"{apiUrl}/points", content, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ResultItem>> Query(
         float[] vector,
         string type,
         CancellationToken ct
     )
        {
            var filter = new
            {
                must = new List<object> { new { key = "type", match = new { value = type } } }
            };

            var request = new
            {
                vector,
                filter,
                limit = 3,
                with_payload = true
            };

            var response = await httpClient.PostAsJsonAsync($"{apiUrl}/points/search", request, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<VectorResult>(cancellationToken: ct)
                         ?? throw new Exception("Failed to deserialize response from Qdrant");
            return result.Result;
        }

        public async Task Remove(string id, CancellationToken cancellation)
        {
            var payload = new { points = new[] { id } };
            var response = await httpClient.PostAsJsonAsync($"{apiUrl}/points/delete", payload, cancellation);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateDefaultCollection()
        {
            if (await IsCollectionExists()) return;

            var content = JsonContent.Create(new
            {
                vectors = new { size = VectorSize, distance = "Cosine" }
            });

            var response = await httpClient.PutAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();
        }

        private async Task<bool> IsCollectionExists()
        {
            try
            {
                var response = await httpClient.GetAsync(apiUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


    }
}
