using bookstoreagent.Books.UseCases.Create;
using bookstoreagent.Infrastructure;
using bookstoreagent.Infrastructure.Vectors;
using bookstoreagent.Models;
using bookstoreagent.Workflow;
using bookstoreagent.Workflow.Agents;
using Microsoft.Agents.AI.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace bookstoreagent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            builder.Services.Configure<OpenAiModel>(builder.Configuration.GetSection("OpenAI"));
            builder.Services.AddSingleton(sp =>
            {
                var config=sp.GetService<IOptions<OpenAiModel>>().Value;
                var client= new OpenAIClient(new ApiKeyCredential(config.ApiKey),new OpenAIClientOptions
                {
                    Endpoint=new Uri(config.Endpoint)
                });
                return client;
            });
            builder.Services.AddChatClient(sp =>
            {
                var client=sp.GetService<OpenAIClient>();
                var config=sp.GetService<IOptions<OpenAiModel>>().Value;
                return client.GetChatClient(config.ModelId).AsIChatClient();
            });
            builder.Services.AddEmbeddingGenerator(sp =>
            {
                var client = sp.GetService<OpenAIClient>();
                var config = sp.GetService<IOptions<OpenAiModel>>().Value;
                return client.GetEmbeddingClient(config.EmbeddingModelId).AsIEmbeddingGenerator();
            });

            builder.Services.AddScoped<BookRecommendationAgentBuilder>();
            builder.Services.AddScoped<IBookRecommendationService,BookRecommendationService>();
           // builder.Services.AddSingleton<IConversationSessionStore,InMemoryConversationSessionStore>();

            builder.Services.AddScoped<BookOrderAgentBuilder>();
            builder.Services.AddScoped<IBookOrderService,BookOrderService>();
            builder.Services.AddScoped<OrderBookWorkflowBuilder>();
            builder.Services.AddScoped<BookOrderWorkflowRunner>();
            builder.Services.AddScoped<IConversationStore, DbConversationStore>();

            builder.Services.AddDbContext<BookAgentDbContext>(options =>
            {
                var conn = builder.Configuration.GetConnectionString("default");
                options.UseMySql(conn,ServerVersion.AutoDetect(conn));
            });

            builder.Services.AddScoped<IEmbedding,OpenAiEmbedding>();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<IVectorDbClient>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                return new QdrantClient(httpClient, builder.Configuration.GetSection("VectorDb:ApiUrl").Value!);
            });
            builder.Services.AddScoped<IBookRepository, EfBookRepository>();
            builder.Services.AddScoped<IVectorBookRepository, VectorBookRepository>();
            builder.Services.AddScoped<CreateBookHandler>();


            var app = builder.Build();
            //await using (var scope = app.Services.CreateAsyncScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<BookAgentDbContext>();
            //    var vectorDbClient = scope.ServiceProvider.GetRequiredService<IVectorDbClient>();

            //    await dbContext.Database.MigrateAsync();
            //    await vectorDbClient.CreateDefaultCollection();
            //}


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
              //  app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

           // app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
