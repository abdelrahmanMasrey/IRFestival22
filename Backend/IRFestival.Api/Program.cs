using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using IRFestival.Api;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppSettingsOptions>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<FestivalDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
sqlServerOptionsAction:sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(
       maxRetryCount: 10,
       maxRetryDelay: TimeSpan.FromSeconds(30),
       errorNumbersToAdd: null
       );
}));

//keyvault
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://irfestivalkeyvaultabd.vault.azure.net/"),
        new DefaultAzureCredential(new DefaultAzureCredentialOptions()));
}

//appconfig
//builder.Configuration.AddAzureAppConfiguration(builder.Configuration.GetConnectionString("AppConfigConnection"));

//activate when sub is enabled
/*builder.Configuration.AddAzureAppConfiguration( option=> 
option.Connect(builder.Configuration.GetConnectionString("AppConfigConnection")).UseFeatureFlags());
builder.Services.AddFeatureManagement();*/


builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
//storage
var storageSharedKeyCredential = new StorageSharedKeyCredential(
    builder.Configuration.GetValue<string>("Storage:AccountName"),
    builder.Configuration.GetValue<string>("Storage:AccountKey"));
string blobUri = "https://" + storageSharedKeyCredential.AccountName + ".blob.core.windows.net";




builder.Services.AddSingleton(p=> new BlobServiceClient(new Uri(blobUri),storageSharedKeyCredential));
builder.Services.AddSingleton(p => storageSharedKeyCredential);
builder.Services.AddSingleton<BlobUtility>();
builder.Services.Configure<BlobSettingsOptions>(builder.Configuration.GetSection("Storage"));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

// THIS IS NOT A SECURE CORS POLICY, DO NOT USE IN PRODUCTION
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
