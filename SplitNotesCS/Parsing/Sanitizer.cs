using Ganss.XSS;
using System;

namespace SplitNotesCS.Parsing
{
    class Sanitizer
    {
        /// <summary>
        /// This class handles the various formatters and sanitizers that are designed to remove
        /// any 'unsafe' HTML and convert Markdown files to HTML
        /// </summary>

        private HtmlSanitizer sanitizer;

        public Sanitizer()
        {
            sanitizer = new HtmlSanitizer();
        }

        /// <summary>
        /// Given input text and a file extension, prepare the text appropriately for HTML rendering.
        /// </summary>
        /// <param name="rawText">Text to be formatted/sanitized</param>
        /// <param name="fileExt">File extension (including ".")</param>
        /// <returns></returns>
        public string Prepare(string rawText, string fileExt)
        {
            string cleanHtml;

            switch (fileExt) {
                case ".md":
                    cleanHtml = PrepareMarkdown(rawText);
                    break;
                case ".html":
                    cleanHtml = PrepareHtml(rawText);
                    break;
                default:
                    cleanHtml = PrepareText(rawText);
                    break;
            }

            return cleanHtml;
            
        }

        /// <summary>
        /// Convert Markdown input to HTML output (also sanitizes because markdown can contain HTML)
        /// </summary>
        /// <param name="rawMarkdown">Markdown input</param>
        /// <returns>Converted Markdown</returns>
        public string PrepareMarkdown(string rawMarkdown)
        {
            string dangerousText = Markdig.Markdown.ToHtml(rawMarkdown);
            string processedText = this.sanitizer.Sanitize(dangerousText);
            return processedText;
        }

        /// <summary>
        /// Convert raw text to basic HTML (preserve line breaks)
        /// Also Sanitizes just in case.
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        public string PrepareText(string rawText)
        {
            string dangerousString = String.Join("<br/>\n", rawText.Split('\n'));
            string outputString = this.sanitizer.Sanitize(dangerousString);
            return outputString;
        }

        /// <summary>
        /// Sanitize HTML and remove possibly dangerous tags.
        /// </summary>
        /// <param name="dangerousHtml">Raw HTML</param>
        /// <returns>Cleaned HTML</returns>
        public string PrepareHtml(string dangerousHtml)
        {
            string safeHtml = this.sanitizer.Sanitize(dangerousHtml);
            return safeHtml;
        }
    }
}
