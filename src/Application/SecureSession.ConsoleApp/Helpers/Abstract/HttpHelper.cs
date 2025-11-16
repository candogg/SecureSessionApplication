using SecureSession.ConsoleApp.Extensions;
using SecureSession.ConsoleApp.Helpers.Base;
using System.Net.Http.Headers;
using System.Text;

namespace SecureSession.ConsoleApp.Helpers.Abstract
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class HttpHelper : HelperBase<HttpHelper>
    {
        private static readonly HttpClientHandler httpClientHandler = new();

        private static readonly HttpClient httpClient = new(httpClientHandler);

        public async Task<TResponse?> PostAsync<TResponse, TRequest>(TRequest requestItem, string url, CancellationToken continuationToken)
        {
            var resultItem = Activator.CreateInstance<TResponse>();

            if (requestItem == null) return resultItem;

            var body = requestItem.Serialize();

            if (body.IsNullOrEmpty()) return resultItem;

            try
            {
                var buffer = Encoding.UTF8.GetBytes(body!);

                using var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var result = await httpClient.PostAsync(url, byteContent, continuationToken).ConfigureAwait(false);

                var postResult = await result.Content.ReadAsStringAsync(continuationToken).ConfigureAwait(false);

                if (postResult.IsNullOrEmpty()) return default;

                resultItem = postResult.Deserialize<TResponse>();

                return resultItem;
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }

            return default;
        }

        public async Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken continuationToken)
        {
            try
            {
                using var result = await httpClient.GetAsync(url, continuationToken).ConfigureAwait(false);

                var postResult = await result.Content.ReadAsStringAsync(continuationToken).ConfigureAwait(false);

                if (postResult.IsNullOrEmpty()) return default;

                var resultItem = Activator.CreateInstance<TResponse>();
                resultItem = postResult.Deserialize<TResponse>();

                return resultItem;
            }
            catch (TaskCanceledException) { }
            catch (Exception) { }

            return default;
        }
    }
}
