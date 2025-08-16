using LegalStrategyAdvisor.Api.Services;
using LegalStrategyAdvisor.Api.Services.AI;
using LegalStrategyAdvisor.Api.Configuration;
using LegalStrategyAdvisor.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var corsOrigins = builder.Configuration["CORS_ORIGINS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? new[] { "http://localhost:4200" };

// Configure AI Provider options
builder.Services.Configure<AiProviderOptions>(
    builder.Configuration.GetSection(AiProviderOptions.SectionName));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod()));

// Database configuration with fallback
var connectionString = builder.Configuration["POSTGRES_CONNECTION"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is missing");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register AI providers
builder.Services.AddSingleton<MockAiProvider>();
builder.Services.AddScoped<OpenAiProvider>();
builder.Services.AddScoped<AzureOpenAiProvider>();

// Register Python AI client with typed HttpClient
builder.Services.AddHttpClient<IPythonAiClient, PythonAiClient>(client =>
{
    var pythonAiBaseUrl = builder.Configuration["PythonAI:BaseUrl"] ?? "http://localhost:8001";
    client.BaseAddress = new Uri(pythonAiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<PythonAiProvider>();

// Register AI service
builder.Services.AddScoped<IAiService, AiService>();

// Register application services
builder.Services.AddScoped<ILegalStrategyService, LegalStrategyService>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Welcome to the Legal Strategy Advisor API!");

app.Run();
