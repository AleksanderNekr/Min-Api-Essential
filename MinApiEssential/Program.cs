using Microsoft.OpenApi.Models;
using MinApiEssential;
using MinApiEssential.Test;
using MinApiEssential.Users;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "My custom minimal API"
    });

    options.IncludeXmlComments(Path.Combine(
        AppContext.BaseDirectory,
        $"{typeof(Program).Assembly.GetName().Name}.xml"));
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapTestApi();
app.MapUserApi();

app.Run();
