using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json.Serialization;
using System.Text.Json;
using Votex.Blazor.Components;
using Votex.Blazor.Models;
using Votex.Blazor.Services;
using Votex.Blazor.HubServices;

namespace Votex.Blazor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddBlazorBootstrap();

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
        builder.Services.AddScoped<JsonSerializerOptions>(_ =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return options;
        });
        builder.Services.AddScoped<ResultsHubService>();
        builder.Services.AddScoped<Authenticator>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<Authenticator>());

        builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, options =>
        {
            options.BaseAddress = new Uri("https://localhost:6969/");
        });

        builder.Services.AddScoped<IVotingsAPIService, VotingsAPIService>();
        builder.Services.AddScoped<IUserAPIService, UserAPIService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
