using System.Net;
using System.Net.Http.Json;

namespace HomeInventory.Client.Tests
{
    internal sealed class MockHomeInventoryApi
    {
        private readonly Dictionary<string, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> _routes = new();

        public List<string> Requests { get; } = [];

        public HttpClient CreateClient()
        {
            var handler = new MockHandler(this);
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost/")
            };
        }

        public void MapJson(HttpMethod method, string relativePathAndQuery, object body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Map(method, relativePathAndQuery, (_, _) => Task.FromResult(CreateJsonResponse(body, statusCode)));
        }

        public void MapStatus(HttpMethod method, string relativePathAndQuery, HttpStatusCode statusCode)
        {
            Map(method, relativePathAndQuery, (_, _) =>
            {
                var response = new HttpResponseMessage(statusCode);
                return Task.FromResult(response);
            });
        }

        public void Map(HttpMethod method, string relativePathAndQuery, Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
        {
            _routes[BuildKey(method, relativePathAndQuery)] = responder;
        }

        private static string BuildKey(HttpMethod method, string relativePathAndQuery)
        {
            return $"{method.Method} {relativePathAndQuery.TrimStart('/')}";
        }

        private static HttpResponseMessage CreateJsonResponse(object body, HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = JsonContent.Create(body)
            };

            return response;
        }

        private sealed class MockHandler(MockHomeInventoryApi api) : HttpMessageHandler
        {
            private readonly MockHomeInventoryApi _api = api;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var pathAndQuery = request.RequestUri?.PathAndQuery.TrimStart('/') ?? string.Empty;
                var key = $"{request.Method.Method} {pathAndQuery}";
                _api.Requests.Add(key);

                if (_api._routes.TryGetValue(key, out var responder))
                {
                    return await responder(request, cancellationToken);
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }
    }
}
