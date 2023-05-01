using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Tests.Helpers;

public class HtmlHelper
{
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(content);

        return document;
    }
}
