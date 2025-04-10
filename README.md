# Adista Angular #2
Formation de démarrage sur Angular pour débutants - Partie 2

## Prérequis

Pour implémenter l'authentification Azure, il est nécessaire d'enregistrer l'application sur Azure, dans **[App registrations](https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade)** <br/>
Documentation : [Inscrire une application avec la plateforme d’identités Microsoft](https://learn.microsoft.com/fr-fr/entra/identity-platform/howto-call-a-web-api-with-curl?tabs=dotnet6%2Cbash&pivots=no-api) <br/>
Pour notre test local, il faut exposer une Api pour un scope défini et enregistrer l'URI local pour Angular : http://localhost:4200

## 1. Implémenter l'authentification Azure dans l'Api

Ajouter le package Microsoft.Identity.Web

Ajouter la configuration Azure dans appsettings.json :

``` JSON
  "AzureAd": {   
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "#{AzureAdTenantId}#",
    "ClientId": "#{AzureAdClientId}#"
  },
  "AllowedHosts": "*"
```

Dans program.cs, ajouter la configuration *Microsoft Identity Web* :

``` Csharp
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
```

Et protéger le controller avec l'annotation *Authorize* :

``` Csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationsController : ControllerBase
```

Tester dans Swagger : vous avez maintenant une erreur 401 😊

## 2. Configurer l'authentification dans Swagger

Parce que c'est pratique !!

Dans program.cs, remplacer ``builder.Services.AddSwaggerGen();`` par :

``` Csharp
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
```

Et remplacer ``app.UseSwagger(); app.UseSwaggerUI();`` par :

``` Csharp
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
```

Tester dans Swagger en vous connectant : vous accéder maintenant à l'Api 😊

## 3. Implémenter l'authentification Azure dans Angular

Récupérer les sources du template Adenes qui contient déjà toute la configuration Angular pour Azure ...
