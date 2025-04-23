using System.Text;
using System.Text.Json;
using CartService.Interfaces;

namespace CartService.Services;

public class OrderHttpClient : IOrderClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderHttpClient> _logger;

    public OrderHttpClient(HttpClient httpClient, ILogger<OrderHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<object?> CreateOrderAsync(Guid userId, List<OrderItemDto> items)
    {
        var payload = new
        {
            userId,
            items
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/orders", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[Order] Misslyckades skapa order. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Order] Undantag vid skapande av order");
            return null;
        }
    }
}