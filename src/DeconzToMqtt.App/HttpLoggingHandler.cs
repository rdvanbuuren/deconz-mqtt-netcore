using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt
{
    /// <summary>
    /// Logs http client request/response.
    /// </summary>
    /// <remarks>
    /// Credits: https://github.com/reactiveui/refit/issues/258#issuecomment-243394076
    /// </remarks>
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly string[] _types = new[] { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };
        private readonly ILogger<HttpLoggingHandler> _logger;

        public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) : base()
        {
            _logger = logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} - Request]";

            _logger.LogDebug($"{msg}========Start==========");
            _logger.LogDebug($"{msg} {request.Method} {request.RequestUri.PathAndQuery} {request.RequestUri.Scheme}/{request.Version}");
            _logger.LogDebug($"{msg} Host: {request.RequestUri.Scheme}://{request.RequestUri.Host}");

            foreach (var header in request.Headers)
                _logger.LogDebug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                    _logger.LogDebug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

                if (request.Content is StringContent || IsTextBasedContentType(request.Headers) || IsTextBasedContentType(request.Content.Headers))
                {
                    var result = await request.Content.ReadAsStringAsync();

                    _logger.LogDebug($"{msg} Content:");
                    _logger.LogDebug($"{msg} {string.Join("", result.Cast<char>().Take(255))}");
                }
            }

            var start = DateTime.Now;
            var response = await base.SendAsync(request, cancellationToken);
            var end = DateTime.Now;

            _logger.LogDebug($"{msg} Duration: {end - start}");
            _logger.LogDebug($"{msg}==========End==========");

            msg = $"[{id} - Response]";
            _logger.LogDebug($"{msg}=========Start=========");

            _logger.LogDebug($"{msg} {request.RequestUri.Scheme.ToUpper()}/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

            foreach (var header in response.Headers)
                _logger.LogDebug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                    _logger.LogDebug($"{msg} {header.Key}: {string.Join(", ", header.Value)}");

                if (response.Content is StringContent || IsTextBasedContentType(response.Headers) || IsTextBasedContentType(response.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await response.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    _logger.LogDebug($"{msg} Content:");
                    _logger.LogDebug($"{msg} {string.Join("", result.Cast<char>().Take(255))}");
                    _logger.LogDebug($"{msg} Duration: {end - start}");
                }
            }

            _logger.LogDebug($"{msg}==========End==========");
            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out IEnumerable<string> values))
                return false;
            var header = string.Join(" ", values).ToLowerInvariant();

            return _types.Any(t => header.Contains(t));
        }
    }
}