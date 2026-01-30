using ApiGymAutomationN8N.Config;
using ApiGymAutomationN8N.Interface;
using ApiGymAutomationN8N.Models;
using ApiGymAutomationN8N.Service;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var nameCors = "AllowFrontEnd";

builder.Services.Configure<HeaderAuthConfig>(builder.Configuration.GetSection("HeaderAuthConfig"));
builder.Services.Configure<N8nConfig>(builder.Configuration.GetSection("N8nConfig"));

builder.Services.AddHttpClient<IN8nService, N8nService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(nameCors, policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unexpected",
            message = "An internal API error occurred. Please try again later."
        });
    });
});

app.UseCors(nameCors);

app.MapPost("/api/send-workout", async ([FromBody] WorkoutRequest request, [FromServices] IN8nService serivce) =>
{
    var response = await serivce.SendRequestToN8N(request);

    if (response == null || response.Status == "Error") return Results.Json(response, statusCode: 500);

    return Results.Ok(response);
});

app.Run();
