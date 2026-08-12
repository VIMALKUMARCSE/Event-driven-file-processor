using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using EventDrivenFileProcessor.Data;
using EventDrivenFileProcessor.Models;
using EventDrivenFileProcessor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddScoped<S3Service>();
builder.Services.Configure<CognitoSettings>(
    builder.Configuration.GetSection("AWS"));

builder.Services.AddScoped<CognitoAuthService>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddAWSService<IAmazonDynamoDB>();

builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddScoped<FileHistoryService>();
builder.Services.AddBlazoredLocalStorage();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });
builder.Services.AddSingleton<WeatherForecastService>();

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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
