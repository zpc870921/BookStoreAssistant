namespace bookstoreagent.Models
{
    public class OpenAiModel
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string ModelId { get; set; }
        public string EmbeddingModelId { get; set; }
    }
}
