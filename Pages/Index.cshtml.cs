using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoClient.Models;
using TodoClient.Services;

namespace TodoClient.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, TodoApiService todoApiService) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly TodoApiService _todoApiService = todoApiService;

        public List<TodoItem> TodoItems { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public string? Action { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? NewTitle { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            _logger.LogInformation("Action={}", Action ?? "[default]");

            try
            {
                switch (Action)
                {
                    case "Create":
                        await _todoApiService.CreateItem(NewTitle);
                        return RedirectToPage();
                    case "Switch":
                        await _todoApiService.SwitchItem(Id);
                        return RedirectToPage();
                    case "Delete":
                        await _todoApiService.DeleteItem(Id);
                        return RedirectToPage();
                    default:
                        TodoItems = await _todoApiService.GetItems();
                        break;
                }
            }
            catch (ArgumentException e)
            {
                TodoItems = await _todoApiService.GetItems();
                ErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error message: {}", e.Message);
                return RedirectToPage("/Error");
            }

            return Page();
        }
    }
}
