using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinApiEssential;
using MinApiEssential.Data;
using MinApiEssential.Extensions;
using MinApiEssential.Resources;
using MinApiEssential.Test;
using MinApiEssential.Users;
using MinApiEssential.Users.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "My API",
            Description = "My custom minimal API",
        });

    options.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory,
        $"{typeof(Program).Assembly.GetName().Name}.xml"));

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        }
    });

    options.AddTagDescriptionHandler();
});

builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme,
        options =>
        {
            options.LoginPath = "/auth/login";
            options.ReturnUrlParameter = "return";
            options.AccessDeniedPath = "/";
        })
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services
    .AddAuthorization()
    .AddAuthorizationBuilder()
    .AddPolicy(
        IdentityConstants.ApplicationScheme,
        policyBuilder => policyBuilder
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme)
            .RequireAuthenticatedUser())
    .AddPolicy(
        IdentityConstants.BearerScheme,
        policyBuilder => policyBuilder
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, IdentityConstants.BearerScheme)
            .RequireAuthenticatedUser());

builder.Services.AddIdentityCore<User>(options =>
    {
        if (!builder.Environment.IsDevelopment())
        {
            return;
        }
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 3;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints()
    .AddErrorDescriber<ErrorDescriber>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    CultureInfo[] supportedCultures = [ new("ru"), new("en") ];
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

app.UseRequestLocalization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.RouteTemplate = "api/{documentName}/swagger.json");
    app.UseSwaggerUI(options => options.RoutePrefix = "api");
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapTestApi();
app.MapUserApi();
app.MapAuthApi();

app.Run();