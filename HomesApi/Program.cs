using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Microsoft Identity Web App services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
// Add Microsoft Identity Scope for API
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("access_as_user", policy =>
    {
        policy.RequireScope("access_as_user");
    });
});

// Add services to the container.

// Ajouter les services CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger   
//builder.Services.AddSwaggerGen();
var azureAdConfig = builder.Configuration.GetSection("AzureAd");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Adenes.ElloAdjusters", Version = "1.0" });
    c.AddSecurityDefinition("oauth2",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{azureAdConfig.GetValue<string>("TenantId")}/oauth2/v2.0/authorize"),
                    Scopes = new Dictionary<string, string>
                    {
                                { $"api://{azureAdConfig.GetValue<string>("ClientId")}/{azureAdConfig.GetValue<string>("ApiScope")}", "Access to your API" }
                    }
                }
            }
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" } },
                    new[] { $"api://{azureAdConfig.GetValue<string>("ClientId")}/{azureAdConfig.GetValue<string>("ApiScope")}" }
                }
            });

    c.CustomSchemaIds(type => type.FullName);
});


var app = builder.Build();

// Appliquer la politique CORS
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    var azureAdClientId = builder.Configuration["AzureAd:ClientId"] ?? string.Empty;
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1");
        c.OAuthClientId(azureAdClientId);
        c.OAuthScopes($"api://{azureAdClientId}/access_as_user");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
