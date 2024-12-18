using System.Text;
using System.Text.Json;

namespace Prema.Services.StorageHub.Workers
{
    public class GoogleAuthTokenRefreshService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly HttpClient _httpClient;
        private readonly TokenStore _tokenStore;

        public GoogleAuthTokenRefreshService(TokenStore tokenStore)
        {
            _httpClient = new HttpClient();
            _tokenStore = tokenStore;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the token refresh every 60 minutes
            _timer = new Timer(RefreshToken, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }

        private async void RefreshToken(object state)
        {
            var url = "https://developers.google.com/oauthplayground/refreshAccessToken";

            var requestBody = new
            {
                token_uri = "https://oauth2.googleapis.com/token",
                client_id = "446582447081-kvn54ekfd6hlfntbr9a8ebig6nvglh0e.apps.googleusercontent.com",
                client_secret = "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",
                refresh_token = "1//040GgdrnJ8uP7CgYIARAAGAQSNwF-L9IrmUT0nUWISKxVvTQfGgmtGaTG_66LkOS1DD_snBnymYlcc9IgdUIYSYpu6i5-uXSZ1KY"
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Extract and update the token in the TokenStore
                    var newToken = responseJson.GetProperty("access_token").GetString();
                    _tokenStore.SetAccessToken(newToken);

                    Console.WriteLine($"Access token refreshed: {newToken}");
                }
                else
                {
                    Console.WriteLine($"Error refreshing token: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during token refresh: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _httpClient?.Dispose();
        }
    }

    public class TokenStore
    {
        private string _accessToken;
        private string _refreshToken;
        private readonly object _lock = new();

        public string GetAccessToken()
        {
            lock (_lock)
            {
                return _accessToken;
            }
        }

        public void SetAccessToken(string token)
        {
            lock (_lock)
            {
                _accessToken = token;
            }
        }

        public string GetRefreshToken()
        {
            lock (_lock)
            {
                return _refreshToken;
            }
        }

        public void SetRefreshToken(string token)
        {
            lock (_lock)
            {
                _refreshToken = token;
            }
        }
    }


}