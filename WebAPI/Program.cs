using System.Reflection;
using System.Text.Json;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

bool IsValidJson(string strInput)
{
    strInput = strInput.Trim();
    if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && // For object
        (!strInput.StartsWith("[") || !strInput.EndsWith("]"))) return false; // For array
    try
    {
        JsonSerializer.Deserialize<object>(strInput);
        return true;
    }
    catch
    {
        // ignored
    }

    return false;
}

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Where(e => IsValidJson(e.ErrorMessage))
                .Select(e => JsonSerializer.Deserialize<Error>(e.ErrorMessage)).ToList();

            if (!errors.Any())
                errors = null;

            var response = new ResponseModel<string?>
            {
                IsSuccess = false,
                ValidationErrors = errors
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddFluentValidation(config => { config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); });

builder.Services.AddTransient<IValidatorInterceptor, UseCustomErrorModelInterceptor>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ResponseModel<string?>
            {
                IsSuccess = false,
                Message = contextFeature.Error.Message,
                ValidationErrors = null
            }));
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();