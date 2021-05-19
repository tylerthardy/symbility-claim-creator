using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SymbilityClaimAccess.Models.Configuration;

namespace SymbilityClaimAccess
{
    public class SymbilityApiHttpClient : HttpClient
    {
        private readonly SymbilityOAuthManager _oAuthManager;

        public SymbilityApiHttpClient(SymbilityApiConfiguration configuration)
        {
            BaseAddress = new Uri(configuration.ApiUrl);
            _oAuthManager = new SymbilityOAuthManager(configuration);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            DefaultRequestHeaders.Authorization = await GetAuthHeader();
            return await base.SendAsync(request);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            DefaultRequestHeaders.Authorization = await GetAuthHeader();
            return await base.SendAsync(request, cancellationToken);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            DefaultRequestHeaders.Authorization = await GetAuthHeader();
            return await base.SendAsync(request, completionOption);
        }

        public new async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            DefaultRequestHeaders.Authorization = await GetAuthHeader();
            return await base.SendAsync(request, completionOption, cancellationToken);
        }

        private async Task<AuthenticationHeaderValue> GetAuthHeader()
        {
            var token = await _oAuthManager.GetTokenAsync();
            var header = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return header;
        }

        internal class SymbilityOAuthManager
        {
            private readonly string _clientSecret;
            private readonly string _clientId;
            private readonly string _accessTokenUrl;
            private const string GrantType = "client_credentials";

            private AuthToken _token;
            private DateTime? _tokenExpiration;

            public SymbilityOAuthManager(SymbilityApiConfiguration configuration)
            {
                _accessTokenUrl = configuration.ApiUrl + "/authentication/token";
                _clientSecret = configuration.Secret;
                _clientId = configuration.ClientId;
            }

            public async Task<AuthToken> GetTokenAsync()
            {
                if (_token != null && _tokenExpiration != null && _tokenExpiration > DateTime.UtcNow)
                {
                    return _token;
                }

                using var client = GetAuthClient();
                try
                {
                    var request = await client.PostAsync(_accessTokenUrl, GetBody());
                    if (!request.IsSuccessStatusCode)
                    {
                        var errorResponse = await request.Content.ReadAsStringAsync();
                        throw new Exception($"Unsuccessful response fetching Symbility OAuth Token from {_accessTokenUrl}: {(int)request.StatusCode} {request.StatusCode} - {errorResponse}");
                    }
                    var response = await request.Content.ReadAsStringAsync();
                    var token = JsonSerializer.Deserialize<AuthToken>(response);
                    _token = token;
                    _tokenExpiration = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
                    return _token;
                }
                catch (Exception ex)
                {
                    throw new Exception($"There was an issue retrieving an oauth token: {ex.Message} / {ex.InnerException?.Message}\r\n{ex.StackTrace}");
                }
            }

            private HttpClient GetAuthClient()
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = GetAuthHeader();
                return client;
            }

            private AuthenticationHeaderValue GetAuthHeader()
            {
                var credentials = $"{_clientId}:{_clientSecret}";
                var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                return new AuthenticationHeaderValue("Basic", base64Credentials);
            }

            private FormUrlEncodedContent GetBody()
            {
                var requestData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", GrantType)
                };
                var requestBody = new FormUrlEncodedContent(requestData);
                return requestBody;
            }

            internal class AuthToken
            {
                [JsonPropertyName("access_token")]
                public string AccessToken { get; set; }
                [JsonPropertyName("expires_in")]
                public int ExpiresIn { get; set; }
                [JsonPropertyName("token_type")]
                public string TokenType { get; set; }
            }
        }
    }
}