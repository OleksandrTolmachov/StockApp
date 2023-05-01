namespace Tests.Helpers;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsFormUrlEncodedAsync<T>(this HttpClient httpClient, string? requestUri, T? content)
    {
        var dict = ObjectToDict(content);
        var formContent = dict is not null ? new FormUrlEncodedContent(dict) : null;
        return httpClient.PostAsync(requestUri, formContent);
    }

    private static IEnumerable<KeyValuePair<string, string>>? ObjectToDict<T>(T? value)
    {
        return value?.GetType().GetProperties()
            .ToList()
            .Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(value)?.ToString() ?? ""));
    }
}
