using Backend.Models;
using Microsoft.AspNetCore.Html;

namespace Backend.Html;

public static class HtmlFactory
{
    public static HtmlString GenerateHtml(ContentPart part)
    {
        switch (part.Type)
        {
            case ContentPartType.Paragraph:
                return GenerateParagraph(part.Content);
            case ContentPartType.Heading1:
                return GenerateHeading1(part.Content);
            case ContentPartType.Heading2:
                return GenerateHeading2(part.Content);
            case ContentPartType.Heading3:
                return GenerateHeading3(part.Content);
            case ContentPartType.Heading4:
                return GenerateHeading4(part.Content);
            case ContentPartType.Image:
                if (string.IsNullOrWhiteSpace(part.Link))
                {
                    throw new ArgumentException("Link needs to be a url set!");
                }
                return GenerateImage(part.Link);
            case ContentPartType.Link:
                if (string.IsNullOrWhiteSpace(part.Link))
                {
                    throw new ArgumentException("Link needs to be a url set!");
                }
                return GenerateLink(part.Link, part.Content);
            case ContentPartType.Video:
                if (string.IsNullOrWhiteSpace(part.Link))
                {
                    throw new ArgumentException("Link needs to be a url set!");
                }
                return GenerateVideo(part.Link);
            default:
                throw new NotSupportedException();
        }
    }

    private static HtmlString GenerateParagraph(string content) => new($"<p class=\"post-paragraph\">{content.Replace("\r\n", "<br />")}</p>");
    
    private static HtmlString GenerateHeading1(string content) => new($"<h1 class=\"post-heading1\">{content.Replace("\r\n", " ")}</h1>");
    
    private static HtmlString GenerateHeading2(string content) => new($"<h2 class=\"post-heading2\">{content.Replace("\r\n", " ")}</h2>");

    private static HtmlString GenerateHeading3(string content) => new($"<h3 class=\"post-heading3\">{content.Replace("\r\n", " ")}</h3>");

    private static HtmlString GenerateHeading4(string content) => new($"<h4 class=\"post-heading4\">{content.Replace("\r\n", " ")}</h4>");
    
    private static HtmlString GenerateImage(string link) => new($"<img src=\"{link}\" class=\"post-image\"/>");
    
    private static HtmlString GenerateLink(string link, string content) => new($"<a href=\"{link}\" class=\"post-webmark\">{content}</a>");
    
    private static HtmlString GenerateVideo(string link) => new($"<video src=\"{link}\" class=\"post-video\"/>");
}