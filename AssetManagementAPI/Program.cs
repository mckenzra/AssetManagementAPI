using AssetManagementAPI.Extensions;
using AssetManagementAPI.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.Net.Http.Headers;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using System.Text.Json.Serialization;

#region Add Services
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

builder.Services.AddSwaggerGen(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins((builder.Configuration.GetValue<string>("AllowedOrigins", "") ?? "").Split(";"))
            .WithHeaders(HeaderNames.Origin, HeaderNames.XRequestedWith, HeaderNames.ContentType, HeaderNames.Accept);
    });
});

builder.Services.AddKeycloakAuth(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ConfigureForNodaTime(NodaTime.DateTimeZoneProviders.Tzdb);
    });

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCustomProblemDetails();

builder.Services.AddEndpointsApiExplorer();

// New repositories and validators should be registered inside these service extensions.
builder.Services.AddRepository();
builder.Services.AddValidator();

builder.Services.AddEFCore(builder.Configuration);
#endregion

#region Build App
var app = builder.Build();

app.Logger.LogInformation("Starting up server");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();