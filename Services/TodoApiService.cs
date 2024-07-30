using System.Text.Json;
using System.Web.Http;
using TodoClient.Models;

namespace TodoClient.Services
{
    public class TodoApiService(ILogger<TodoApiService> logger, IHttpClientFactory httpClientFactory)
    {
        private static readonly JsonSerializerOptions s_jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        private readonly ILogger<TodoApiService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<List<TodoItem>> GetItems()
        {
            var httpResponseMessage = await CreateHttpClient().GetAsync("");
            _logger.LogInformation("Response code on getting items: {}", httpResponseMessage.StatusCode);
            CheckHttpResponseStatus(httpResponseMessage);

            var content = GetHttpResponseContent(httpResponseMessage).Result;
            _logger.LogInformation("Got items: {}", content);

            return JsonSerializer.Deserialize<List<TodoItem>>(content, s_jsonOptions) ?? ([]);
        }

        public async Task<TodoItem> CreateItem(string? title)
        {
            _logger.LogInformation("New title: {}", title);
            CheckTitleForEmpty(title);
            TodoItem newTodoItem = new() { Title = title! };
            _logger.LogInformation("Prepared new item: {}", JsonSerializer.Serialize(newTodoItem));

            var httpResponseMessage = await CreateHttpClient().PostAsJsonAsync("", newTodoItem);
            _logger.LogInformation("Response code on creating item: {}", httpResponseMessage.StatusCode);
            CheckHttpResponseStatus(httpResponseMessage);

            var content = GetHttpResponseContent(httpResponseMessage).Result;
            _logger.LogInformation("Created item: {}", content);

            return JsonSerializer.Deserialize<TodoItem>(content, s_jsonOptions) ?? (new TodoItem());
        }

        public async Task SwitchItem(string? id)
        {
            _logger.LogInformation("Id: {}", id);
            CheckIdForNumeric(id);
            var httpResponseMessage = await CreateHttpClient().PatchAsync(id, null);
            _logger.LogInformation("Response code on switching item with id={}: {}", id, httpResponseMessage.StatusCode);
            CheckHttpResponseStatus(httpResponseMessage);
        }

        public async Task DeleteItem(string? id)
        {
            _logger.LogInformation("Id: {}", id);
            CheckIdForNumeric(id);
            var httpResponseMessage = await CreateHttpClient().DeleteAsync(id);
            _logger.LogInformation("Response code on deleting item with id={}: {}", id, httpResponseMessage.StatusCode);
            CheckHttpResponseStatus(httpResponseMessage);
        }

        private HttpClient CreateHttpClient()
        {
            return _httpClientFactory.CreateClient("TodoApiHttpClient");
        }

        private static async Task<string> GetHttpResponseContent(HttpResponseMessage httpResponseMessage)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            StreamReader reader = new(contentStream);
            string content = reader.ReadToEnd();
            return content ?? "";
        }

        private static void CheckHttpResponseStatus(HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        private static void CheckTitleForEmpty(string? title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Title should not be empty!");
            }
        }

        private static void CheckIdForNumeric(string? id)
        {
            if (!int.TryParse(id, out _))
            {
                throw new ArgumentException("Id should be numeric!");
            }
        }
    }
}
