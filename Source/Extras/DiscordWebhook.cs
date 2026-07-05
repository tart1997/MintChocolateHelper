namespace Celeste.Mod.MintChocolateHelper.Extras;

public class DiscordWebhook
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;

    internal DiscordWebhook(string webhookUrl)
    {
        _webhookUrl = webhookUrl;
        _httpClient = new HttpClient();
    }

    internal async Task SendMessageAsync(string content)
    {
        StringContent httpContent = new(content, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_webhookUrl, httpContent);
        response.EnsureSuccessStatusCode();
    }
}