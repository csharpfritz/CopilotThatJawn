using Microsoft.AspNetCore.Http.Extensions;
using Web.Extensions;
using Web.Models;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // Add controller support for API endpoints
builder.Services.AddMvc().AddViewComponentsAsServices(); // Register view components

// Register content service
builder.Services.AddScoped<IContentService, ContentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers(); // Map controller routes for API endpoints

// Map sitemap and RSS feed endpoints
app.MapSitemapEndpoint();
app.MapRssFeedEndpoint();

app.Run();
