using Microsoft.Net.Http.Headers;
using TodoClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("TodoApiHttpClient", httpClient =>
{
    //Set "TodoApiUrl" in appsettings.json like "http://localhost:5006/api/TodoItems"
    httpClient.BaseAddress = new Uri(builder.Configuration["TodoApiUrl"]!);
    httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "text/plain");
});
builder.Services.AddTransient<TodoApiService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
